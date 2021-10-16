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

    [HideInInspector] public bool attack;
    private bool landed;

    [SerializeField] GameObject sword;
    [SerializeField] InverseKinematics ikHandler;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        animator.SetLookAtPosition(cameraYawRotater.transform.forward);
        


        //animation handling
        animator.SetBool("Grounded",motor.Grounded);
        animator.SetFloat("Speed",rigidbody.velocity.magnitude);
        animator.SetFloat("Forward speed", moveInput.y);
        animator.SetFloat("Horizontal speed", moveInput.x);
        //state machine to handle the particles
        if(motor.Grounded && !landed)
        {
            particle.Emit(10);
        }
        landed = motor.Grounded;
    }
    void OnFire(InputValue input)
    {
        attack = true;
        animator.SetBool("Attacking", true);
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
}
