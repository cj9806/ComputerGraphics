using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Sample character script demonstrating how to send inputs to a motor
/// </summary>
public class SamplePlayerCharacter : MonoBehaviour
{
    public Vector3 startingPoint;
    [SerializeField] PlayerInput playerInput;
    // The motor we're controlling
    public KinematicPlayerMotor motor;
    public ParticleSystem particle;
    public Animator animator;
    public Rigidbody rigidbody;

    //mouse contols
    [SerializeField]GameObject cameraPitchRotater;
    public GameObject cameraYawRotater;
    public float rotationSpeed;
    public float lookMax;
    private float pitchControl = 0f;
    private float minPitch = -26;
    private float maxPitch = 60;
    private Mouse mouse;

    [HideInInspector] public bool attacking;
    private bool landed;

    public GameObject sword;
    [HideInInspector]
    public bool canAttack = true;
    [SerializeField] InverseKinematics ikHandler;

    public Camera camera;
    private bool blinkAiming = false;
    private bool blink = false;
    public float maxBlinkDistance;
    public LayerMask blinkmask;

    //player stats
    [Header("Player Stats")]
    public float health = 100;
    public float stamina;
    public float mana;
    [SerializeField] HudController hud;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        mouse = Mouse.current;
    }
    private void Update()
    {
        Vector2 moveInput = playerInput.currentActionMap["Move"].ReadValue<Vector2>();
        
        Vector3 newMovInp = new Vector3(moveInput.x, 0, moveInput.y);
        newMovInp = cameraYawRotater.transform.rotation * newMovInp;
        // send inputs to motor
        motor.MoveInput(newMovInp);
        if (playerInput.currentActionMap["Jump"].ReadValue<float>() == 1)
        {
            motor.JumpInput();
            
        }

        Vector2 lookInput = playerInput.currentActionMap["Look"].ReadValue<Vector2>();
        //rigidbody.rotation = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f, lookInput.x, 0));
        pitchControl = Mathf.Clamp(pitchControl - lookInput.y * rotationSpeed, minPitch, maxPitch);
        cameraYawRotater.transform.localRotation = cameraYawRotater.transform.localRotation * Quaternion.AngleAxis(lookInput.x, Vector3.up);
        cameraPitchRotater.transform.localRotation = Quaternion.Euler(pitchControl, 0, 0);
        //get mouse postition
        //animator.SetLookAtPosition(cameraYawRotater.transform.forward);

        //blink handling
        if (blinkAiming && mana > 25)
        {
            Ray ray = camera.ScreenPointToRay(mouse.position.ReadValue());
            bool hitGround = Physics.Raycast(ray, out RaycastHit hitInfo, maxBlinkDistance,blinkmask,QueryTriggerInteraction.Ignore);
            Vector3 blinkPoint;
            if (hitGround) blinkPoint = ray.origin + ray.direction * hitInfo.distance;
            else blinkPoint = ray.origin + ray.direction * maxBlinkDistance;
            particle.transform.position = blinkPoint;
            particle.Emit(10);
            if (blink)
            {
                transform.position = blinkPoint;
                blinkAiming = false;
                blink = false;
                mana -= 25;
            }
        }
        if(mana < 100)
        {
            mana += .05f;
        }
        
        //animation handling
        animator.SetBool("Grounded",motor.Grounded);
        animator.SetFloat("Speed",rigidbody.velocity.magnitude);
        animator.SetFloat("Forward speed", moveInput.y);
        animator.SetFloat("Horizontal speed", moveInput.x);


        //hupdate stats
        if (stamina < 100)
            stamina += .025f;
        hud.health = health;
        hud.stamina = stamina;
        hud.mana = mana;
        

        if(health <= 0)
        {
            //gameover
        }
    }
    private void FixedUpdate()
    {
        if (transform.position.y < -25)
        {
            playerInput.transform.position = startingPoint;
            rigidbody.velocity = Vector3.zero;
        }
        //check to see if the attack animation is done playing before i can attack again
        canAttack = !animator.GetBool("Attacking");
    }
    void OnFire(InputValue input)
    {
        if (canAttack && stamina > 25)
        {
            attacking = true;
            animator.SetBool("Attacking", true);
            stamina -= 25;
        }
    }
    void OnSwitchWeapon(InputValue input)
    {

        if (sword.activeSelf == false)
        {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 1.0f);
            ikHandler.unarmed = false;
        }
        else
        {
            animator.SetLayerWeight(1, 1.0f);
            animator.SetLayerWeight(2, 0);
            ikHandler.unarmed = true;
        }
        sword.SetActive(!sword.activeSelf);
    }
    void OnBlink(InputValue input)
    {
        if (mana >= 25)
        {
            if (input.Get<float>() == 1) blinkAiming = true;
            if (input.Get<float>() == 0) blink = true;
        }
    }
    
}
