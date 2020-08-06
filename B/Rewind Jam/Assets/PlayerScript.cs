using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class PlayerScript : MonoBehaviour
{
    private Collision coll;
    [HideInInspector] public Rigidbody2D rb;
    private LevelManager lvlManager;
    private Animator anim;
    public SpriteRenderer spriteHolder;
    private EndLevel endLevelScript;

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
    [HideInInspector] public float xRaw;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool canDoubleJump = true;
    private bool hasDashed = false;
    public bool isDashing;
    public float dashDistance;
    private bool groundTouch;
    public bool isOutsideDash;
     public bool facingRight = true;

    [Space]
    [Header("Particle Effects")]
    public ParticleSystem runningDust, jumpDust;
    public ParticleSystem dashDust;

    public Material playerMaterial;

    [Space]

    public GameObject timeRift;

    public GameObject[] timeRifts;
    
    
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lvlManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        timeRifts = new GameObject[lvlManager.timeRiftLimits];
        endLevelScript = GameObject.FindGameObjectWithTag("EndLevel").GetComponent<EndLevel>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        anim.SetFloat("YVelocity", rb.velocity.y);

        if (rb.velocity.x != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
                Jump(Vector2.up);
            else if (!coll.onGround && canDoubleJump)
            {
                Jump(Vector2.up);
                InstantiateTimeRift(0);
                CamShake(.1f, 2f, 14, 90);
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
            anim.SetBool("OnGround", true);
            groundTouch = true;
        }
        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
            anim.SetBool("OnGround", false);
        }

        CheckDash(x);

        if (isOutsideDash)
        {
            OutsideDash(isOutsideX);
        }


        if(facingRight && x < 0)
        {
            Debug.Log("MAuq");
            Flip();
        }
        else if(!facingRight && x > 0)
        {
            Flip();
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
        anim.Play("StartJump");
        CreateJumpDust();
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
        endLevelScript.CheckLevelStatus();
        StartCoroutine(CheckStatusCD());
    }


    // -----------------------------------------------------------------------

    private void CheckDash(float x)
    {
        if (isDashing)
        {
            GetComponent<BetterJumping>().enabled = false;
            if (dashTimeLeft > 0)
            {
                anim.SetBool("isDashing", true);
                canMove = false;
                rb.velocity = new Vector2(dashSpeed * x, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
            }
        }

        if(dashTimeLeft <= 0)
        {
            anim.SetBool("isDashing", false);
            isDashing = false;
            canMove = true;
        }
    }
    void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        CreateDashDust();
        CamShake(.2f, 3f, 14, 90);
        if (coll.onGround == false)
        {
            anim.SetBool("isDashing", true);
            anim.Play("Dash");
        }
    }

    public void OutsideDash(float x)
    {
        //isOutsideDash = true;       

        CreateDashDust();
        endLevelScript.CheckLevelStatus();
        GetComponent<BetterJumping>().enabled = false;       
        if (isOutsideDash)
        {
            GetComponent<BetterJumping>().enabled = false;
            if (dashTimeLeft > 0)
            {
                canMove = false;
                rb.velocity = new Vector2((dashSpeed + dashSpeedOutsideOffset) * x, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
            }
        }

        if (dashTimeLeft <= 0)
        {
            isOutsideDash = false;
            canMove = true;
        }
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
        CreateJumpDust();
        if (GetComponent<TimeBody>().isRewinding == false)
        {
            StartCoroutine(BetterJumpingCD());
        }
    }


    public void InstantiateTimeRift(int typeTxt)
    {
        if(timeRifts[lvlManager.actualTimeRifts] != null)
        {
            Destroy(timeRifts[lvlManager.actualTimeRifts]);
            timeRifts[lvlManager.actualTimeRifts] = null;
        }
        timeRifts[lvlManager.actualTimeRifts] = Instantiate(timeRift, transform.position, transform.rotation);
        timeRifts[lvlManager.actualTimeRifts].GetComponent<SpriteRenderer>().sprite = spriteHolder.sprite;
        timeRifts[lvlManager.actualTimeRifts].GetComponent<SpriteRenderer>().flipX = spriteHolder.flipX;
        timeRifts[lvlManager.actualTimeRifts].GetComponent<TimeRiftScript>().type = typeTxt;

        lvlManager.actualTimeRifts++;

        if (lvlManager.actualTimeRifts >= lvlManager.timeRiftLimits)
        {
            lvlManager.actualTimeRifts = 0;
        }

        endLevelScript.CheckLevelStatus();
    }

    void Flip()
    {
        facingRight = !facingRight;
        spriteHolder.flipX = !facingRight;

        CreateRunningDust();
    }


    void CreateRunningDust()
    {
        runningDust.Play();
    }

    void CreateJumpDust()
    {
        jumpDust.Play();
    }

    void CreateDashDust()
    {
        dashDust.Play();
    }

    IEnumerator BetterJumpingCD()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<BetterJumping>().enabled = true;
    }

    IEnumerator CheckStatusCD()
    {
        yield return new WaitForSeconds(.06f);
        endLevelScript.CheckLevelStatus();
    }

    public void CamShake(float duration, float strenght, int vibrato, int randomness)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(duration, strenght, vibrato, randomness, false, true);
    }
}
