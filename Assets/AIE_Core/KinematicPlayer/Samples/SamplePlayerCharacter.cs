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
    [Header("Motor references")]
    public KinematicPlayerMotor motor;
    public ParticleSystem particle;
    public Animator animator;
    public Rigidbody rigidbody;

    //mouse contols
    [Header("Camera Controls")]
    [SerializeField] GameObject cameraPitchRotater;
    public GameObject cameraYawRotater;
    public float rotationSpeed;
    public float lookMax;
    private float pitchControl = 0f;
    private float minPitch = -26;
    private float maxPitch = 60;
    private Mouse mouse;
    public Camera camera;

    //attack handling
    [Header("Attacks")]
    [HideInInspector] public bool attacking;
    public GameObject sword;
    [HideInInspector]
    public bool canAttack = true;
    [SerializeField] InverseKinematics ikHandler;
    [SerializeField] GameObject punchTarget;
    //magic
    [Header("Magic")]
    private bool blinkAiming = false;
    private bool blink = false;
    public float maxBlinkDistance;
    public LayerMask blinkmask;


    public GameObject magicMissle;

    private bool activeSpell; //true=blink false=magic missle

    //player stats
    [Header("Player Stats")]
    public float health = 100;
    public float stamina;
    public float mana;
    public float manRegenSpeed;
    public float stamRegenSpeed;

    [Header("UI References")]
    [SerializeField] HudController hud;
    [SerializeField] GameObject FistIcon;
    [SerializeField] GameObject SwordIcon;
    [SerializeField] GameObject blinkIcon;
    [SerializeField] GameObject missleIcon;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject deathScreen;
    public bool hasWon;
    private bool paused;
    private void Start()
    {
        hasWon = false;
        paused = false;
        activeSpell = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouse = Mouse.current;
        Time.timeScale = 1;
    }
    private void Update()
    {
        if (!paused)
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
            if (transform.position.y < -25)
            {
                motor.body.InternalVelocity = Vector3.zero;
                this.transform.position = startingPoint;                
            }
            //check to see if the attack animation is done playing before i can attack again
            canAttack = !animator.GetBool("Attacking");
            Vector2 lookInput = playerInput.currentActionMap["Look"].ReadValue<Vector2>();
            //rigidbody.rotation = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f, lookInput.x, 0));
            pitchControl = Mathf.Clamp(pitchControl - lookInput.y * rotationSpeed, minPitch, maxPitch);
            cameraYawRotater.transform.localRotation = cameraYawRotater.transform.localRotation * Quaternion.AngleAxis(lookInput.x * rotationSpeed, Vector3.up);
            cameraPitchRotater.transform.localRotation = Quaternion.Euler(pitchControl, 0, 0);

            //get mouse postition
            //animator.SetLookAtPosition(cameraYawRotater.transform.forward);

            //blink handling
            if (blinkAiming && mana > 25)
            {
                Ray ray = camera.ScreenPointToRay(mouse.position.ReadValue());
                bool hitGround = Physics.Raycast(ray, out RaycastHit hitInfo, maxBlinkDistance, blinkmask, QueryTriggerInteraction.Ignore);
                Vector3 blinkPoint;                
                ParticleSystem.MainModule main = particle.main;
                if (hitInfo.collider != null)
                {
                    main.startColor = Color.green;
                }
                else main.startColor = Color.blue;
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
            if (mana < 100)
            {
                mana += .05f * manRegenSpeed;
            }

            //animation handling
            animator.SetBool("Grounded", motor.Grounded);
            animator.SetFloat("Speed", rigidbody.velocity.magnitude);
            animator.SetFloat("Forward speed", moveInput.y);
            animator.SetFloat("Horizontal speed", moveInput.x);


            //hupdate stats
            if (stamina < 100)
                stamina += .05f * stamRegenSpeed;
            hud.health = health;
            hud.stamina = stamina;
            hud.mana = mana;


            if (health <= 0)
            {
                paused = true;
                Time.timeScale = 0;
                deathScreen.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (hasWon)
            {
                paused = true;
                Time.timeScale = 0;
                winScreen.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
    private void FixedUpdate()
    {
       
    }
    void OnFire(InputValue input)
    {
        if (canAttack && stamina > 25 && !paused)
        {
            attacking = true;
            animator.SetBool("Attacking", true);
            stamina -= 25;
        }
    }
    void OnSwitchWeapon(InputValue input)
    {
        if (!paused)
        {

            if (sword.activeSelf == false)
            {
                animator.SetLayerWeight(1, 0);
                animator.SetLayerWeight(2, 1.0f);
                ikHandler.unarmed = false;
                SwordIcon.SetActive(true);
                FistIcon.SetActive(false);
            }
            else
            {
                animator.SetLayerWeight(1, 1.0f);
                animator.SetLayerWeight(2, 0);
                ikHandler.unarmed = true;
                SwordIcon.SetActive(false);
                FistIcon.SetActive(true);
            }
            sword.SetActive(!sword.activeSelf);
        }
    }
    void OnMagic(InputValue input)
    {
        if (!paused)
        {
            if (activeSpell)//true is blink is active
            {
                if (mana >= 25)
                {
                    if (input.Get<float>() == 1) blinkAiming = true;
                    if (input.Get<float>() == 0) blink = true;
                }
            }
            else
            {
                if (mana >= 10 && input.Get<float>() == 0)
                {

                    GameObject mm = Instantiate(magicMissle);
                    mm.transform.up = camera.transform.forward;
                    mm.transform.position = camera.ViewportToWorldPoint(new Vector3(.5f, .5f, 3.25f));
                    mana -= 20;
                }

            }
        }
    }
    void OnSwitchSpell(InputValue input)
    {
        if (!paused)
        {
            activeSpell = !activeSpell;
            if (activeSpell)
            {
                blinkIcon.SetActive(true);
                missleIcon.SetActive(false);
            }
            else
            {
                blinkIcon.SetActive(false);
                missleIcon.SetActive(true);
            }
        }
    }
    void OnEscape(InputValue input)
    {
        if (health > 0 && !hasWon)
        {
            if (!pauseMenu.activeInHierarchy)
            {
                paused = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}