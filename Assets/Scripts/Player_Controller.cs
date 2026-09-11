using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.ComponentModel;
using NUnit.Framework;
using UnityEngine.Events;
using Unity.Mathematics;

public class Player_Controller : MonoBehaviour
{   
    // Private variables
    private Animator anim;
    // private PlayerHealth playerHealth;

    // The input component on the player gameobject
    private PlayerInput playerInput;

    // The player actions
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction attackAction;
    private InputAction lockOnAction;
    private InputAction dodgeRollAction;
    private Rigidbody rb;

    [Header("Attack")]
    [SerializeField] private float moveDelay;
    public bool canMove;
    private bool isAttacking;
    public int weaponDamage;
    public UnityEvent PlayerAttack;

    [Header("Move")]
    [SerializeField] private float walkSpeed;
    private Vector3 moveVector;
    private bool isMoving;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float lockMoveAfterLandTime = .5f;
    private bool isSprinting;
    public float currentSpeed;
    [SerializeField] private float transTimeToSprint;
    private bool changingFloat;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private float rotSpeed = .2f;
    private float animFloatX;
    private float animFloatY;
    private bool canRot;

    [Header("Look")]
    [SerializeField] private GameObject cameraBoom;
    [SerializeField] private float sensitivity;
    private Vector2 lookVector;
    private float yRotation;
    private float xRotation;
    
    [SerializeField] private int yMin = -85;
    [SerializeField] private int yMax = 85;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float waitForJumpTime = .2f;
    private float timePassed;
    private bool canJump;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        lookAction = playerInput.actions["Look"];
        lookAction.performed += Look;
        lookAction.canceled += Look;

        moveAction = playerInput.actions["Move"];
        moveAction.performed += Move;
        moveAction.canceled += Move;

        jumpAction = playerInput.actions["Jump"];
        jumpAction.started += Jump;

        sprintAction = playerInput.actions["Sprint"];
        sprintAction.started += SprintPerformed;
        sprintAction.canceled += SprintCanceled;
    }

    private void Start()
    {
        canMove = true;
        currentSpeed = walkSpeed;
        changingFloat = false;
        canRot = true;
        
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
 
    private void Look(InputAction.CallbackContext context)
    {
        // Debug.Log("Look");
            
        lookVector = context.ReadValue<Vector2>() * sensitivity;
    }

    private void Move(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        isMoving = moveVector != Vector3.zero;
    }
    
    private void SprintPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
        currentSpeed = sprintSpeed;

        if(moveVector == Vector3.zero)
            return;
        
        StartCoroutine(ChangeFloatOverTime(.5f, 1, transTimeToSprint));
        
    }

    private void SprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
        currentSpeed = walkSpeed;

        if(moveVector == Vector3.zero)
            return;

        StartCoroutine(ChangeFloatOverTime(1, .5f, transTimeToSprint));
    }
    
    private bool lastIsGrounded = false;
    private bool isLanded = false;

    private void FixedUpdate()
    {   
        bool currentIsGrounded = IsGrounded();

        if (currentIsGrounded && !lastIsGrounded)
        {
            isLanded = true; 
            StartCoroutine(LockMoveLand());
        }

        lastIsGrounded = currentIsGrounded;

        // Reset flag when airborne again
        if (!currentIsGrounded)
        {
            isLanded = false;
        }
            
            
        PerformMove();
        PerformLook();
        
        if(moveVector != Vector3.zero)
        {
            RotatePlayer();
        }

        SetAnimationParams();
    }

    private bool landed;

    private void SetAnimationParams()
    {
        bool runBool = moveVector != Vector3.zero;
        anim.SetBool("Run", runBool);
        
        anim.SetBool("Grounded", IsGrounded());
    }

    private IEnumerator ChangeFloatOverTime(float initialValue, float targetValue, float duration)
    {
        changingFloat = true;

        float elapsedTime = 0f;
        //animFloat = initialValue;

        while (elapsedTime < duration)
        {
            //animFloat = Mathf.Lerp(initialValue, targetValue, elapsedTime / duration);
            //anim.SetFloat("MovementFloat", Mathf.Abs(animFloat));

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        yield return changingFloat = false;  
    }

    private void Update()
    {
        // Canceling sprint if player stops moving
        if(moveVector == Vector3.zero)
        {
            currentSpeed = walkSpeed;
            isSprinting = false;
        }

        // Jumping cooldown
        timePassed += Time.deltaTime;
        if(timePassed > waitForJumpTime)
        {
            canJump = true;
        }

        // Making sure the player can't run backwards   
        if(moveVector.y < -.5)
        {
            if(isSprinting)
            {
                currentSpeed = walkSpeed;
            }            
        }
    }

    private void PerformMove()
    {
        if(canMove == false)
            return;

        // Transform the movement vector to be relative to the camera's orientation
        Vector3 forward = cameraBoom.transform.forward;
        Vector3 right = cameraBoom.transform.right;

        // Ensure the forward and right vectors are horizontal
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate the desired movement direction
        Vector3 desiredMoveDirection = forward * moveVector.y + right * moveVector.x;

        // Apply the movement
        rb.MovePosition(transform.position + currentSpeed * Time.deltaTime * desiredMoveDirection);
    }

    private void PerformLook()
    {
        float mouseX = lookVector.x;
        float mouseY = lookVector.y;

        // Apply horizontal rotation to the player
        //cameraBoom.transform.Rotate(0f, mouseX, 0f);
        xRotation += mouseX;

        // Apply vertical rotation to the camera
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, yMin, yMax);
        cameraBoom.transform.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }
    

    private void Jump(InputAction.CallbackContext context)
    {
        if(IsGrounded() == true && canJump == true)
        {
            //anim.SetTrigger("Jump");
            Vector3 jumpVector = new(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            rb.AddForce(jumpVector, ForceMode.Impulse);
            timePassed = 0f;
            anim.SetTrigger("Jump");
        }
    }

    private void RotatePlayer()
    {   
        Vector3 forward = cameraBoom.transform.forward;
        Vector3 right = cameraBoom.transform.right;

        // Ensure the forward and right vectors are horizontal
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredRotDir = forward * moveVector.y + right * moveVector.x;

        // If there's no input, don't change the rotation
        if (desiredRotDir.sqrMagnitude > 0.0f)
        {
            // Create the desired rotation
            // Apply the rotation to the character
            StartCoroutine(SmoothRotate(desiredRotDir));
        }
    }

    private IEnumerator SmoothRotate(Vector3 targetVector)
    {
        float elapsedTime = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(targetVector);
        Quaternion initialRotation = characterTransform.localRotation;

        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        while (elapsedTime < rotSpeed)
        {
            characterTransform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is set
        characterTransform.localRotation = targetRotation;
    }

    private bool IsGrounded()
    {
        Vector3 castOrigin = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        
        if(Physics.Raycast(castOrigin, Vector3.down, castDistance + 0.2f, groundLayer))
            return true;
        else
            return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (IsGrounded())
            Gizmos.color = Color.green;
        
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), new Vector3(transform.position.x, transform.position.y - castDistance, transform.position.z));
    }

    private IEnumerator LockMoveLand()
    {
        canMove = false;
        yield return new WaitForSeconds(lockMoveAfterLandTime);
        canMove = true;
    }
}