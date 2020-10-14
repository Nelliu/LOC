using Assets.Scripts.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public Controller target;
    public Vector2 focusingAreaSize;
    private FocusArea focus;
    public float VerticalOffset; // offset how much above in Y will camera start looking (like so it doesn't have half of screen just ground..)

    public float lookAheadDistanceX;
    public float smoothTime;
    public float vetricalSmoothTime;
    private float currentLookAheadX;
    private float targetLookAheadX;
    private float lookAheadDirectionX;
    private float smoothlookVelocityX;
    private float smoothvelocityY;

    private bool lookAheadstop;
    void Start()
    {
        focus = new FocusArea(target.collider.bounds, focusingAreaSize);

    }

    void LateUpdate()
    {
        focus.Update(target.collider.bounds);

        Vector2 focusPosition = focus.Center + Vector2.up * VerticalOffset;

        if(focus.velocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focus.velocity.x);
            if (Mathf.Sign(target.PlayerInput.x) == Mathf.Sign(focus.velocity.x) && target.PlayerInput.x != 0)
            {
                lookAheadstop = false;
                targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
            }
            else
            {
                if (!lookAheadstop)
                {
                    targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDirectionX - currentLookAheadX) / 4f;
                    lookAheadstop = true;
                }
                
            }
        }
       
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothlookVelocityX, smoothTime);
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothvelocityY, vetricalSmoothTime);
        //focusPosition += Vector2.right * currentLookAheadX; // needs some work 

        

        transform.position = (Vector3)focusPosition + Vector3.forward * -10;

    }

    void OnDrawGizmos()
    {



        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focus.Center, focusingAreaSize);

    }


}
