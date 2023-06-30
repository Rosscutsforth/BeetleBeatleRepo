using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    //Movement Variables
    public float baseSpeed = 1.5f;
    public float currentSpeed;
    private Vector2 moveAmount;
    private float input;

    //Sprint Variables
    public float sprintIncrease = 1f;
    public float sprintSpeed;

    //jumping variables
    private bool isGrounded = true;
    public float jumpForce = 1.5f;
    public float groundCheckRadius;
    public Transform groundCheck;
    public LayerMask ground;
    private bool isJumping;

    //charge jump variables
    public float maxChargeDuration = 2f;
    private float chargeDuration;
    public float chargedJumpForce;

    //wing flap variables
    public int wingFlapCharges = 3;
    private int currentWingFlapCharges;
    public float wingFlapBoost = 2.5f;

    //wing glide variables
    private float wingGlideSpeed;
    public float wingGlideDescentSpeed = .7f;
    private float keyHoldLength;

    //Animator Variables
    private Animator anim;

    //camera variables
    private Camera mainCamera;
    private float originalCameraSize;
    public float newCameraSize;
    public float cameraTransitionDuration = 2.5f;
    private float currentTime = 0f;
    private float groundedTimer;

    //Rigidbody Variable
    private Rigidbody2D rb;

    //Boxcollider variable
    private BoxCollider2D boxCol2D;

    //crouching variables
    private float originalBoxColOffsetY;
    private float originalBoxColSizeY;
    //variable to check if there is something above the player
    public Transform standUpCheck;
    public float standUpCheckRadius;
    public LayerMask standUpObstructions;
    private bool notClearToStand = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCol2D = GetComponent<BoxCollider2D>();
        originalBoxColOffsetY = boxCol2D.offset.y;
        originalBoxColSizeY = boxCol2D.size.y;

        mainCamera = Camera.main;
        originalCameraSize = mainCamera.orthographicSize;

        wingGlideSpeed = baseSpeed * 2.5f;
        currentSpeed = baseSpeed;
        sprintSpeed = baseSpeed + sprintIncrease;
    }

    // Update is called once per frame
    void Update()
    {

        horizontalMovementAnimations();

        chargeJump();

        wingFlap();

        jumpMovement();

        wingGlide();

        //sprint function call
        Sprint();

        //crouch function call
        Crouch();

        skyCamera();

    }

    void FixedUpdate()
    {

        //horizontal movement
        input = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(input * currentSpeed, rb.velocity.y);

        //checks for ground contact
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);

        //checks for obstructions above the player's head
        notClearToStand = Physics2D.OverlapCircle(standUpCheck.position, standUpCheckRadius, standUpObstructions);
    }

    public void horizontalMovementAnimations()
    {
        //Controls player running animation
        if (input != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        //reflects the player sprite depending on whether they are moving right or left
        if (input > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }else if (input < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
            //anim.SetBool("isSprinting", true);
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = baseSpeed;
            //anim.SetBool("isSprinting", false);
        }
        if(isGrounded == true && Input.GetKey(KeyCode.LeftShift) == false)
        {
            currentSpeed = baseSpeed;
        }
        if(isGrounded == true && Input.GetKey(KeyCode.LeftShift) == true)
        {
            currentSpeed = sprintSpeed;
        }
    }

    public void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            boxCol2D.size = new Vector2(boxCol2D.size.x, boxCol2D.size.y * .5f);
            boxCol2D.offset = new Vector2(boxCol2D.offset.x, boxCol2D.offset.y / .3f);
            anim.SetBool("isCrouching", true);
        }
        if (Input.GetKeyUp(KeyCode.CapsLock) && notClearToStand == false)
        {
            boxCol2D.size = new Vector2(boxCol2D.size.x, originalBoxColSizeY);
            boxCol2D.offset = new Vector2(boxCol2D.offset.x, originalBoxColOffsetY);
            anim.SetBool("isCrouching", false);
        }
        if (Input.GetKey(KeyCode.CapsLock) == false && notClearToStand == false)
        {
            boxCol2D.size = new Vector2(boxCol2D.size.x, originalBoxColSizeY);
            boxCol2D.offset = new Vector2(boxCol2D.offset.x, originalBoxColOffsetY);
            anim.SetBool("isCrouching", false);
        }
    }

    public void chargeJump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            chargeDuration += Time.deltaTime;
            chargeDuration = Mathf.Clamp(chargeDuration, 0, maxChargeDuration);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGrounded == true)
        {
            chargedJumpForce = (chargeDuration + 1) * jumpForce;
            rb.velocity = Vector2.up * chargedJumpForce;
            chargeDuration = 0f;
        }
    }

    public void jumpMovement()
    {
        //jumping
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            isGrounded = false;
            //anim.SetBool("isJumping", true);
            rb.velocity = Vector2.up * jumpForce;
            chargedJumpForce = 0;
        }

        if(isGrounded == true)
        {
            //anim.SetBool("isJumping", false);
        }
    }

    public void wingFlap()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded == false && currentWingFlapCharges > 0)
        {
            if(rb.velocity.y < 0.3f)
            {
                rb.velocity = Vector2.up * wingFlapBoost;
                currentWingFlapCharges -= 1;
            }else
            {
                rb.velocity += Vector2.up * wingFlapBoost;
                currentWingFlapCharges -= 1;
            }
        }

        if(isGrounded == true)
        {
            currentWingFlapCharges = wingFlapCharges;
        }
    }

    public void wingGlide()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            keyHoldLength += Time.deltaTime;
        }else
        {
            keyHoldLength = 0;
        }

        if(Input.GetKey(KeyCode.Space) && isGrounded == false)
        {
            currentSpeed = wingGlideSpeed;
            if(keyHoldLength > .5f)
            {
                rb.gravityScale = .3f;
            }
        }else
        {
            rb.gravityScale = 1f;
        }
    }

    public void skyCamera()
    {
        if(isGrounded == false)
        {
            groundedTimer += Time.deltaTime;
        }else
        {
            groundedTimer = 0f;
        }

        currentTime += Time.deltaTime;
        float t = Mathf.Clamp01(currentTime / cameraTransitionDuration);

        if(groundedTimer > 1.5f)
        {
            print("I'm working");

            float interpolatedSize = Mathf.Lerp(mainCamera.orthographicSize, newCameraSize, t);

            /*Rect interpolatedRect = new Rect(
                Mathf.Lerp(originalCameraRect.x, newCameraRect.x, t),
                Mathf.Lerp(originalCameraRect.y, newCameraRect.y, t),
                Mathf.Lerp(originalCameraRect.width, newCameraRect.width, t),
                Mathf.Lerp(originalCameraRect.height, newCameraRect.height, t)
            );*/

            mainCamera.orthographicSize = interpolatedSize;
        }else
        {

            float reverseInterpolatedSize = Mathf.Lerp(mainCamera.orthographicSize, originalCameraSize, t);

            mainCamera.orthographicSize = reverseInterpolatedSize;
        }
    }

}
