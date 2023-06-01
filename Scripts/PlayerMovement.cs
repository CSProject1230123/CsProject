using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator animator;
    private SpriteRenderer sprite;

    [SerializeField] private LayerMask ground;

    public float dirX;
    private bool runArea;

    [HideInInspector] public bool ledgeDetected;

    //remove later
    [SerializeField] private float mvmtSpeed = 2f;
    [SerializeField] private float jmpForce = 3f;

    [Header("Ledge info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;
    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;

    private GameObject attackArea = default;
    private bool attacking = false;
    private float timeToAttack = 0.25f;
    private float timer = 0f;

    private enum MovementState { idle, walking, jump, fall, sit }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Test");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackArea = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        if (!isSitting())
        {
            rb.velocity = new Vector2(dirX * mvmtSpeed, rb.velocity.y);
        }

        if (Input.GetButtonDown("Jump") && isGrounded() && !isSitting())
        {
            rb.velocity = new Vector2(rb.velocity.x, jmpForce);
        }

        if (Input.GetMouseButtonDown(0))
        {
            attack();
        }

        if (attacking)
        {
            timer += Time.deltaTime;
            if (timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
            }
        }


        CheckforLedge();
        UpdateAnimation();
        AnimatorController();
    }
    private bool isGrounded()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, ground);
    }

    private bool isSitting()
    {
        return Input.GetKey("s");
    }
    private void UpdateAnimation()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.walking;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.walking;
            sprite.flipX = true;

        }
        else
        {
            state = MovementState.idle;
        }

        if (state == MovementState.idle)
        {
            if(Input.GetKey("s"))
            {
                state = MovementState.sit;
            }
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.fall;
        }

        animator.SetInteger("State", (int)state);
    }

    private void AnimatorController()
    {
        animator.SetBool("canClimb", canClimb);
    }

    private void CheckforLedge()
    {
        if(ledgeDetected && canGrabLedge)
        {
            canGrabLedge = false;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;
            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            canClimb = true;
        }

        if (canClimb)
            transform.position = climbBegunPosition;
    }

    private void LedgeClimbOver()
    {
        canClimb = false;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", 1f);
    }

    private void AllowLedgeGrab() => canGrabLedge = true;

    private void attack()
    {
     
    }
}

 