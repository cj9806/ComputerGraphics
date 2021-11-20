using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A fully-kinematic body with callbacks for handling collision and movement
/// </summary>
public class KinematicBody : MonoBehaviour
{
    /// <summary>
    /// Motor driving this body
    /// </summary>
    public IKinematicMotor motor;
    
    [Header("Body Definition")]
#pragma warning disable 0649 // Assigned in Unity inspector
    [SerializeField]
    private CapsuleCollider col;
    public CapsuleCollider BodyCollider => col;
    [SerializeField]
    private Rigidbody rbody;
#pragma warning restore 0649 // Assigned in Unity inspector
    /// <summary>
    /// Size of the box body in local space
    /// </summary>
    public float LocalRadius => col.radius;
    public float LocalHeight => col.height;
    /// <summary>
    /// Minimum desired distance between nearby surfaces and the surface of this body
    /// </summary>
    [FormerlySerializedAs("skinWidth")] public float contactOffset = 0.005f;
    /// <summary>
    /// Size of the box body in local space inclusive of the contact offset
    /// </summary>
    public float LocalRadiusWithSkin => col.radius +  contactOffset;
    public float LocalHeigthWithSkin => col.radius + contactOffset;
    public Vector3 GetLocalOffsetToCenter()
    {
        return col.center;
    }
    public Vector3 GetCenterAtBodyPosition(Vector3 bodyPosition)
    {
        return bodyPosition + col.center;
    }
    /// <summary>
    /// Position of the feet (aka bottom) of the body
    /// </summary>
    public Vector3 FootPosition => transform.TransformPoint(col.center/* + Vector3.down * col.size.y/2.0f*/) - new Vector3();
    /// <summary>
    /// Offset from the pivot of the body to the feet
    /// </summary>
    public Vector3 FootOffset => (FootPosition - transform.position);

    [Header("Body Settings")]
    public Vector3 GravityScale = new Vector3(0, 2, 0);
    // velocity of the final object inclusive of external forces, given in world-space
    public Vector3 EffectiveGravity
    {
        get
        {
            Vector3 g = Physics.gravity;
            g.Scale(GravityScale);
            return g;
        }
    }
    
    public Vector3 InternalVelocity { get; set; }
    public Vector3 Velocity { get; private set; }

    public LayerMask collisionMask;

    public void CollideAndSlide(Vector3 bodyPosition, Vector3 bodyVelocity, Collider other)
    {
        DeferredCollideAndSlide(ref bodyPosition, ref bodyVelocity, other);
        
        // apply movement immediately
        rbody.MovePosition(bodyPosition);
        InternalVelocity = bodyVelocity;
    }

    public void DeferredCollideAndSlide(ref Vector3 bodyPosition, ref Vector3 bodyVelocity, Collider other)
    {
        // ignore self collision
        if(other == col) { return; }
            
        bool isOverlap = Physics.ComputePenetration(col,
            bodyPosition,
            rbody.rotation,
            other,
            other.transform.position,
            other.transform.rotation,
            out var mtv,
            out var pen);
        

        if (isOverlap)
        {
            // defer to motor to resolve hit
            motor.OnMoveHit(ref bodyPosition, ref bodyVelocity, other, mtv, pen);
        }
    }
    //---------TERRY CODE BUILT FOR BOX COLLIDERS----------
    //public Collider[] Overlap(Vector3 bodyPosition, int layerMask = ~0, QueryTriggerInteraction queryMode = QueryTriggerInteraction.UseGlobal)
    //{
    //    bodyPosition = GetCenterAtBodyPosition(bodyPosition);
    //    return Physics.OverlapBox(bodyPosition, LocalBodySize/2, rbody.rotation, layerMask, queryMode);
    //}
    
    //public Collider[] Overlap(Vector3 bodyPosition, Vector3 bodyHalfExtents, int layerMask = ~0, QueryTriggerInteraction queryMode = QueryTriggerInteraction.UseGlobal)
    //{
    //    bodyPosition = GetCenterAtBodyPosition(bodyPosition);
    //    return Physics.OverlapBox(bodyPosition, bodyHalfExtents, rbody.rotation, layerMask);
    //}
    public Collider[] Overlap(CapsuleCollider collider, int layermask = ~0, QueryTriggerInteraction queryMode = QueryTriggerInteraction.UseGlobal)
    {
        //the points are = to top/bottom of the capsule +- the radius
        //top/bottom can be detirmined from collider.center +- 1/2 height;
        Vector3 worldPos = transform.TransformPoint(collider.center);
        Vector3 top = worldPos + new Vector3(0, (collider.height / 2)-col.radius, 0);
        Vector3 bot = worldPos - new Vector3(0, (collider.height / 2)+col.radius, 0);
        return Physics.OverlapCapsule(top,bot,collider.radius,layermask,queryMode);
    }
    
    public RaycastHit[] Cast(Vector3 bodyPosition, Vector3 direction, float distance, int layerMask = ~0, QueryTriggerInteraction queryMode = QueryTriggerInteraction.UseGlobal)
    {
        bodyPosition = GetCenterAtBodyPosition(bodyPosition);
        //var allHits = Physics.BoxCastAll(bodyPosition, LocalBodySizeWithSkin/2, direction, rbody.rotation, distance, layerMask, queryMode);
        Vector3 worldPos = transform.TransformPoint(col.center);
        Vector3 top = worldPos + new Vector3(0, (col.height / 2) - col.radius, 0);
        Vector3 bot = worldPos - new Vector3(0, (col.height / 2) + col.radius, 0);
        var allHits = Physics.CapsuleCastAll(top, bot, col.radius, direction, distance,layerMask,queryMode);

        // TODO: this is terribly inefficient and generates garbage, please optimize this
        List<RaycastHit> filteredhits = new List<RaycastHit>(allHits);
        filteredhits.RemoveAll( x => x.collider == col);
        return filteredhits.ToArray();
    }
    
    //
    // Unity Messages
    //

    private void Start()
    {
        OnValidate();
    }

    private void FixedUpdate()
    {
        Vector3 startPosition = rbody.position;
        
        motor.OnPreMove();
        
        InternalVelocity = motor.UpdateVelocity(InternalVelocity);

        //
        // integrate external forces
        //
        
        // apply gravity (if enabled)
        if (rbody.useGravity)
        {
            InternalVelocity += EffectiveGravity * Time.deltaTime;
        }

        Vector3 projectedPos = rbody.position + (InternalVelocity * Time.deltaTime);
        Vector3 projectedVel = InternalVelocity;
        
        //
        // depenetrate from overlapping objects
        //

        float radiusOriginal = col.radius;
        float radiusWithSkin = col.radius + contactOffset;

        float heightOriginal = col.height;
        float heightWithSkin = col.height + contactOffset;

        var candidates = Overlap(col,collisionMask,QueryTriggerInteraction.Ignore);

        // HACK: since we can't pass a custom size to Physics.ComputePenetration (see below),
        //       we need to assign it directly to the collide prior to calling it and then
        //       revert the change afterwards
        col.radius = radiusWithSkin;
        
        foreach (var candidate in candidates)
        {
            DeferredCollideAndSlide(ref projectedPos, ref projectedVel, candidate);
        }
        
        // HACK: restoring size (see above HACK)
        col.radius = radiusOriginal;
        
        // callback: pre-processing move before applying 
        motor.OnFinishMove(ref projectedPos, ref projectedVel);
        
        // apply move
        rbody.MovePosition(projectedPos);
        InternalVelocity = projectedVel;

        Velocity = (projectedPos - startPosition) / Time.fixedDeltaTime;
        
        // callback for after move is complete
        motor.OnPostMove();
    }

    private void OnValidate()
    {
        contactOffset = Mathf.Clamp(contactOffset, 0.001f, float.PositiveInfinity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        //Gizmos.draw(col.center, col.size + Vector3.one * contactOffset);
    }
}

public interface IKinematicMotor
{
    /// <summary>
    /// Called by KinematicBody when it initially updates its velocity
    /// </summary>
    /// <param name="oldVelocity">Existing velocity</param>
    /// <returns>Returns the new velocity to apply to the body</returns>
    Vector3 UpdateVelocity(Vector3 oldVelocity);

    /// <summary>
    /// Called by KinematicBody when the body hits another collider during its move
    /// </summary>
    /// <param name="curPosition">Position of the body at time of impact</param>
    /// <param name="curVelocity">Velocity of the body at time of impact</param>
    /// <param name="other">The collider that was struck</param>
    /// <param name="direction">Depenetration direction</param>
    /// <param name="pen">Penetration depth</param>
    void OnMoveHit(ref Vector3 curPosition, ref Vector3 curVelocity, Collider other, Vector3 direction, float pen);

    // TODO: Make these callbacks instead of part of the interface
    
    /// <summary>
    /// Called before the body has begun moving
    /// </summary>
    void OnPreMove();
    
    /// <summary>
    /// Called before the move is applied to the body.
    ///
    /// This provides an opportunity to perform any post-processing on the move
    /// before it is applied to the body.
    /// </summary>
    /// <param name="curPosition">Position that the body would move to</param>
    /// <param name="curVelocity">Velocity that the body would move with on next update</param>
    void OnFinishMove(ref Vector3 curPosition, ref Vector3 curVelocity);
    
    /// <summary>
    /// Called after the body has moved to its final position for this frame
    /// </summary>
    void OnPostMove();
}