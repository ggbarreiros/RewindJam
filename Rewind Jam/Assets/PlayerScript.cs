using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerScript : MonoBehaviour
{
    private Collision coll;
    [HideInInspector] public Rigidbody2D rb;
    private LevelManager lvlManager;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float dashSpeed = 20;
    public float dashTime;
    public float dashCooldown;
    public float dashTimeLeft;
    [HideInInspector]public float lastDash = -100;
    public float isOutsideX;
    public float dashSpeedOutsideOffset;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool canDoubleJump = true;
    private bool hasDashed = false;
    public bool isDashing;
    public float dashDistance;
    private bool groundTouch;
    public bool isOutsideDash;

    public GameObject timeRift;
    
    
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        lvlManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
                Jump(Vector2.up);
            else if (!coll.onGround && canDoubleJump)
            {
                Jump(Vector2.up);
                InstantiateTimeRift(0);
                canDoubleJump = false;
            }

        }


        if (Input.GetButtonDown("Dash") && !isDashing)
        {
            if (xRaw != 0 || yRaw != 0)
            {
                if(Time.time >= (lastDash + dashCooldown))
                    AttemptToDash();
                if(x > 0)
                {
                    InstantiateTimeRift(2);
                }
                else
                {
                    InstantiateTimeRift(1);
                }
            }
                //Dash(xRaw, dashTime);
        }


        if(coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }
        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        CheckDash(x);

        if (isOutsideDash)
        {
            OutsideDash(isOutsideX);
        }
    }

    private void LateUpdate()
    {
        if (coll.onGround)
        {
            canDoubleJump = true;
            hasDashed = false;
        }

    }

    public void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    public void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }


    // -----------------------------------------------------------------------

    private void CheckDash(float x)
    {
        if (isDashing)
        {
            GetComponent<BetterJumping>().enabled = false;
            if (dashTimeLeft > 0)
            {
                canMove = false;
                rb.velocity = new Vector2(dashSpeed * x, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
                //Debug.Log("isDashing");
            }
        }

        if(dashTimeLeft <= 0)
        {
            //Debug.Log("isDashing false");
            isDashing = false;
            canMove = true;
        }
    }
    void AttemptToDash()
    {
        //Debug.Log("AttemptToDash");
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
    }

    public void OutsideDash(float x)
    {
        //isOutsideDash = true;       
        

        GetComponent<BetterJumping>().enabled = false;
        if (isOutsideDash)
        {
            GetComponent<BetterJumping>().enabled = false;
            if (dashTimeLeft > 0)
            {
                canMove = false;
                rb.velocity = new Vector2((dashSpeed + dashSpeedOutsideOffset) * x, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
                //Debug.Log("isDashing");
            }
        }

        if (dashTimeLeft <= 0)
        {
            //Debug.Log("isDashing false");
            isOutsideDash = false;
            canMove = true;
        }
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
        if (GetComponent<TimeBody>().isRewinding == false)
        {
            GetComponent<BetterJumping>().enabled = true;
        }
    }


    public void InstantiateTimeRift(int typeTxt)
    {
        if (lvlManager.actualTimeRifts < lvlManager.timeRiftLimits)
        {
            lvlManager.actualTimeRifts++;
            GameObject TR = Instantiate(timeRift, transform.position, transform.rotation);
            TR.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            TR.GetComponent<TimeRiftScript>().type = typeTxt;
        }
    }
    /*
    public void Dash(float x, float time)
    {
        hasDashed = true;
        GetComponent<BetterJumping>().enabled = false;
        float dashVelocity = x * dashSpeed;
        rb.DOMoveX(rb.position.x + dashVelocity * Time.deltaTime, time, true);

        //rb.velocity = new Vector2(rb.velocity.x * dashSpeed, rb.velocity.y) * Time.deltaTime;
    }

    */


}
