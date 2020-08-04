using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeBody : MonoBehaviour
{
    [HideInInspector]public bool isRewinding = false;

    public float recordTime;

    //List<Vector2> positions;
    List<PointInTime> pointsInTime;

    Rigidbody2D rb;
    BoxCollider2D col;
    SpriteRenderer sr;

    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Rewind"))
            StartRewind();
        //if (Input.GetButtonUp("Rewind"))
            //StopRewind();

        if (isRewinding)
            Rewind();
        else
            Record();
    }

    void Record()
    {
        if ((rb.velocity.x > 0.1f || rb.velocity.x < -0.1f) || (rb.velocity.y > 0.1f || rb.velocity.y < -0.1f))
        {
            //if (pointsInTime.Count > Mathf.Round(5f / Time.fixedDeltaTime))
            //{
            //pointsInTime.RemoveAt(pointsInTime.Count - 1);
            //}
            Debug.Log("Recording " + rb.velocity);
            pointsInTime.Insert(0, new PointInTime(transform.position, sr.sprite));
        }
    }

    void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            sr.sprite = pointInTime.sprite;
            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }

    public void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
        col.enabled = false;
        rb.gravityScale = 0f;
        GetComponent<BetterJumping>().enabled = false;
        attTimeRifts();
    }

    public void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
        col.enabled = true;
        rb.gravityScale = 4.87f;
        GetComponent<BetterJumping>().enabled = true;
    }

    void attTimeRifts()
    {
        GameObject[] timerifts = GameObject.FindGameObjectsWithTag("TimeRift");

        for(int i = 0; i < timerifts.Length; i++)
        {
            timerifts[i].GetComponent<TimeRiftScript>().AttRift();
        }
    }
}
