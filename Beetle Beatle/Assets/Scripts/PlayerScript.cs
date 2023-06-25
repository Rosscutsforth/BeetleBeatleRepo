using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    //Movement Variables
    public float moveSpeed = 9f;
    private Vector2 moveAmount;
    private float input;

    //jumping variables
    private bool isGrounded = true;
    public float jumpForce = 1.5f;
    public float groundCheckRadius;
    public Transform groundCheck;
    public LayerMask ground;
    private bool isJumping;

    //Animator Variables
    private Animator anim;

    //Rigidbody Variable
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
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

        //jumping
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            isGrounded = false;
            //anim.SetBool("isJumping", true);
            rb.velocity = Vector2.up * jumpForce;
        }

        if(isGrounded == true)
        {
            //anim.SetBool("isJumping", false);
        }
    }

    void FixedUpdate()
    {

        //horizontal movement
        input = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(input * moveSpeed, rb.velocity.y);

        //checks for ground contact
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
    }

}
