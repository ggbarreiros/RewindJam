using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRiftScript : MonoBehaviour
{
    public int type;
    private bool active = false;

    private void Awake()
    {
        StartCoroutine(InitialCoolDown());
    }



    /// <summary>
    /// 0 - Double Jump
    /// 1 - Left Dash
    /// 2 - Right Dash
    /// </summary>
    /// 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if (active)
            {
                PlayerScript playerScript = other.GetComponent<PlayerScript>();
                switch (type)
                {
                    case 0:
                        other.GetComponent<BetterJumping>().enabled = false;
                        playerScript.Jump(Vector2.up);
                        break;
                    case 1:
                        playerScript.canMove = false;
                        playerScript.isOutsideX = -1f;
                        playerScript.dashTimeLeft = other.GetComponent<PlayerScript>().dashTime;
                        playerScript.lastDash = Time.time;
                        playerScript.isOutsideDash = true;
                        break;
                    case 2:
                        playerScript.canMove = false;
                        playerScript.isOutsideX = 1f;
                        playerScript.dashTimeLeft = other.GetComponent<PlayerScript>().dashTime;
                        playerScript.lastDash = Time.time;
                        playerScript.isOutsideDash = true;
                        break;
                    default:
                        break;
                }
                GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>().actualTimeRifts--;
                Destroy(gameObject);
            }
        }
    }

    IEnumerator InitialCoolDown()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void AttRift()
    {
        active = true;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
