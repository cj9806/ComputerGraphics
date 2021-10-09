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
    public Animator animator;
    public Rigidbody rigidbody;

    //mouse contols
    [SerializeField]GameObject cameraRotater;
    public float rotationSpeed;
    public float lookMax;
    private float pitchControl = 0f;
    private float minPitch = -26;
    private float maxPitch = 60;

    [HideInInspector] public bool punch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        Vector2 moveInput = playerInput.currentActionMap["Move"].ReadValue<Vector2>();
        Vector3 newMovInp = new Vector3(moveInput.x, 0, moveInput.y);
        newMovInp = transform.rotation * newMovInp;
        // send inputs to motor
        motor.MoveInput(newMovInp);
        if (playerInput.currentActionMap["Jump"].ReadValue<float>() == 1)
        {
            motor.JumpInput();

        }

        Vector2 lookInput = playerInput.currentActionMap["Look"].ReadValue<Vector2>();
        rigidbody.rotation = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f, lookInput.x, 0));
        pitchControl = Mathf.Clamp(pitchControl - lookInput.y * rotationSpeed, minPitch, maxPitch);
        cameraRotater.transform.localRotation = Quaternion.Euler(pitchControl, 0, 0);
        


        //animation handling
        animator.SetBool("Grounded",motor.Grounded);
        animator.SetFloat("Speed",rigidbody.velocity.magnitude);
        animator.SetFloat("Forward speed", moveInput.y);
        animator.SetFloat("Horizontal speed", moveInput.x);

    }
    void OnFire(InputValue input)
    {
        punch = true;
    }
}
