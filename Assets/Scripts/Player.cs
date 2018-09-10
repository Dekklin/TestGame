using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D myRigidBody;
    private Animator myAnimator;
    private bool attack;
    private bool slide;
    private bool jump;
    private bool jumpAttack;

    [SerializeField] private float jumpForce;
    [SerializeField] private Transform[] groundPoints;
    [SerializeField] private float movementSpeed;
    private bool isGrounded;
    [SerializeField] private float groundRadius;
    [SerializeField] private bool airControl;

    [SerializeField] private LayerMask whatIsGround;
    private bool facingLeft;


	// Use this for initialization
	void Start ()
    {
        facingLeft = false;
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
	}

    void Update()
    {
        HandleInput();
    }

    private void ResetValues()
    {
        attack = false;
        slide = false;
        jump = false;
        jumpAttack = false;
    }
    // Update is called once per frame
    void FixedUpdate ()
    {
        float horizontal = Input.GetAxis("Horizontal");
        isGrounded = IsGrounded();
        HandleMovement(horizontal);
        HandleAttacks();
        HandleLayers();
        Flip(horizontal);


        ResetValues();
	}
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
            jumpAttack = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            slide = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }


    private void HandleAttacks()
    {
        if (attack && isGrounded && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myAnimator.SetTrigger("Attack");
            myRigidBody.velocity = Vector2.zero;
        }
        if(jump && isGrounded && !this.myAnimator.GetCurrentAnimatorStateInfo(1).IsName("JumpAttack"))
        {
            myAnimator.SetBool("JumpAttack", true);
        }
        if( !jumpAttack && !this.myAnimator.GetCurrentAnimatorStateInfo(1).IsName("JumpAttack"))
        {
            myAnimator.SetBool("JumpAttack", false);
        }
    }

    private void HandleMovement(float horizontal)
    {
        if (myRigidBody.velocity.y < 0)
        {
            myAnimator.SetBool("Land", true);
        }
        if ((isGrounded || airControl) && !myAnimator.GetBool("Slide") && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myRigidBody.velocity = new Vector2(horizontal * movementSpeed, myRigidBody.velocity.y);
        }
        if (isGrounded && jump)
        {
            isGrounded = false;
            myRigidBody.AddForce(new Vector2(0, jumpForce));
            myAnimator.SetTrigger("Jump");
        }
        if (slide && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            myAnimator.SetBool("Slide", true);
        }
        else if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            myAnimator.SetBool("Slide", false);
        }

        myAnimator.SetFloat("Speed", Mathf.Abs(horizontal));
    }

    private bool IsGrounded()
    {
        if(myRigidBody.velocity.y <= 0)
        {
            foreach( Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for(int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject)
                    {
                        myAnimator.ResetTrigger("Jump");
                        myAnimator.SetBool("Land", false);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void Flip(float horizontal)
    {
        if(horizontal > 0 && facingLeft || horizontal < 0 && !facingLeft)
        {
            facingLeft = !facingLeft;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
    private void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    }
}
