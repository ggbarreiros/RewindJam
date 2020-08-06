using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRiftScript : MonoBehaviour
{
    public int type;
    private bool active = false;


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
            if (active && !other.GetComponent<TimeBody>().isRewinding)
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
                        playerScript.isOutsideX = playerScript.xRaw;
                        playerScript.dashTimeLeft = other.GetComponent<PlayerScript>().dashTime;
                        playerScript.CamShake(.2f, 3f, 14, 90);
                        playerScript.lastDash = Time.time;
                        playerScript.isOutsideDash = true;
                        break;
                    case 2:
                        playerScript.canMove = false;
                        playerScript.isOutsideX = playerScript.xRaw;
                        playerScript.dashTimeLeft = other.GetComponent<PlayerScript>().dashTime;
                        playerScript.CamShake(.2f, 3f, 14, 90);
                        playerScript.lastDash = Time.time;
                        playerScript.isOutsideDash = true;
                        break;
                    default:
                        break;
                }
                if (GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>().actualTimeRifts > 0)
                {
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>().actualTimeRifts--;
                }
                else
                {
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>().actualTimeRifts = 3;
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>().actualTimeRifts--;
                }
                Destroy(gameObject);
            }
            else if(!active && other.GetComponent<TimeBody>().isRewinding)
            {
                AttRift();
            }
        }
    }


    public void AttRift()
    {
        active = true;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
