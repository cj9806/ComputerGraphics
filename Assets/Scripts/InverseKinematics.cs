using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InverseKinematics : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    public Transform target;
    private bool retracted = true;
    private float weight;
    [SerializeField] SamplePlayerCharacter player;
    private void OnAnimatorIK(int layerIndex)
    {
        
        //punch when told to
        if (player.punch)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, target.transform.position);
            if(retracted)
            weight += Time.fixedDeltaTime;
        }
        //retract when fully extendend
        if (weight >= 1) retracted = false;
        if (!retracted)
        {
            weight -= Time.fixedDeltaTime;
        }
        if(!retracted&& weight < 0)
        {
            player.punch = false;
            retracted = true;
        }
        //tell that you can punch again
    }
    
}
