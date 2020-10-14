using Assets.Scripts.Movement;
using Assets.Scripts.Movement.Angles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : Raycast
{


    public CollisionInfo collisionInfo = new CollisionInfo();
    internal Vector2 PlayerInput;
    internal bool spikesHit; // knows if spikes were hit or not
    internal bool checkpointHit; // knows if checkpoint collider was hit or not
    internal string nameOfCheckpoint; // stores name in string of last checkpoint reacher
    internal bool canMoveToNext; // signs if player can already move to next level
    public GameObject tutorial; // has object which stores tutorial help
    public GameObject finish; // has object which stores information that shows at end of level
    public GameObject levelHelp; // has object which stores help for another level

    public bool isSquare;

    internal Respawn respawn;
    internal Transform newCheckpoint;
    public override void Start()
    {
        base.Start();
        respawn = GetComponent<Respawn>();
        collisionInfo.directionfacing = 1;
    }


    public void Move(Vector3 velocity, Vector2 Input, bool OnPlatform = false) // this function is called when object moves
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        collisionInfo.oldVelocity = velocity;
        PlayerInput = Input;

        if (velocity.y < 0)
        {
            DescendGround(ref velocity);

        }

        if (velocity.x != 0)
        {
            collisionInfo.directionfacing = (int)Mathf.Sign(velocity.x);
        }

        HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        if (OnPlatform == true)
        {
            collisionInfo.below = true;
        }
           
        


        transform.Translate(velocity);
    }

    public void Move(Vector3 velocity, bool OnPlatform = false) // overload method for platform movement (since there is no input that platform gives)
    {
        Move(velocity, Vector2.zero, OnPlatform);
    }

    private void HorizontalCollisions(ref Vector3 velocity)  // function working with horizontal collisions 
    {
        float directionX = collisionInfo.directionfacing;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }


        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {

                spikesHit = (hit.collider.tag == "Spikes") ? true : false; // if spikes are hit from horizontal

                if (hit.collider.tag == "Checkpoint")
                {
                    checkpointHit = true;
                    nameOfCheckpoint = hit.collider.name;
                    respawn.UpdateCheckPointOnHit();
                    continue;
                }
                //else checkpointHit = false;

                if (hit.collider.tag == "Disable")
                {
                    
                    tutorial.SetActive(false);
                    levelHelp.SetActive(false);
                    continue;
                }
                if (hit.collider.tag == "Finish")
                {
                    finish.SetActive(true);
                    canMoveToNext = true;
                    continue;
                }
           


                if (hit.distance == 0 ) // if hit by platform from side, don't do anything to player, don't stop him from moving
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= Angle.Climbing)
                {
                    if (collisionInfo.descending)
                    {
                        collisionInfo.descending = false; // in case player descends one but hits another from other side == he stops descending
                        velocity = collisionInfo.oldVelocity;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {

                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbGround(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisionInfo.climbing || slopeAngle > Angle.Climbing)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisionInfo.climbing)
                    {
                        velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }

            }
        }
    }


    private void VerticalCollisions(ref Vector3 velocity) // same as HC ^ but with VC
    {

        float directionY = Mathf.Sign(velocity.y); // positive if up, negative if down
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);

            if (hit)
            {
                if(hit.collider.tag == "JumpThrough") /// need to finish this for some horizontal collisions aswel
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisionInfo.fallingThrough)
                    {
                        continue;
                    }
                    if (PlayerInput.y == -1)
                    {
                        collisionInfo.fallingThrough = true;
                        Invoke("ResetFallingTimer", .5f); // without this, player sometimes can't fall down from platform because platform "catches" him as he is falling though
                        continue;
                    }


                   
                }
             
                // if spikes are hit from horizontal
                spikesHit = (hit.collider.tag == "Spikes") ? true : false;

                if (hit.collider.tag == "Checkpoint")
                {
                    checkpointHit = true;
                    nameOfCheckpoint = hit.collider.name;
                    respawn.UpdateCheckPointOnHit();
                    continue;
                }
                //else checkpointHit = false;

                if (hit.collider.tag == "Disable")
                {
                    tutorial.SetActive(false);
                    levelHelp.SetActive(false);
                    continue;
                }
               

                if (hit.collider.tag == "Finish")
                {
                    finish.SetActive(true);
                    canMoveToNext = true;
                    continue;

                }

                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbing)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x); // counting velocity dependant on angle (when floor isnt 0, but going up
                }

                collisionInfo.below = directionY == -1; // if directionX == -1 -> below = true
                collisionInfo.above = directionY == 1;

            }

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.blue);
        }

        if (collisionInfo.climbing) // solving when ground is more complicated so object doesn't go through 
        {
            float dirX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 raycastorig = ((dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(raycastorig, Vector2.right * dirX, rayLength, CollisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisionInfo.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    collisionInfo.slopeAngle = slopeAngle;
                }
            }


        }
    }
    private void ResetFallingTimer() // resets timer on falling through platform
    {
        collisionInfo.fallingThrough = false;
    }


    private void ClimbGround(ref Vector3 velocity, float angle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisionInfo.below = true; // is grounded
            collisionInfo.climbing = true;
            collisionInfo.slopeAngle = angle;
        }
    }

    private void DescendGround(ref Vector3 velocity) // no need to give angle since we get it from y < 0
    {
        float dirX = Mathf.Sign(velocity.x);
        Vector2 rayCasting = (dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft; // if I am moving left, cast from bottom right and otherwise (descending so it needs to be other side than we are moving)
        RaycastHit2D hit = Physics2D.Raycast(rayCasting, Vector2.down, Mathf.Infinity, CollisionMask); // Mathf.infinity == casts all way down if there is something descend to
        
        if (hit)
        {
            float groundAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (groundAngle != 0 && groundAngle <= Angle.Descending)
            {
                if (Mathf.Sign(hit.normal.x) == dirX) // -> means we are moving down
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(groundAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) // if player is actually close to ground (don't want to descend if he is at last block of platform
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float DescendVelocityY = Mathf.Sin(groundAngle * Mathf.Deg2Rad) * moveDistance;

                        velocity.y = -DescendVelocityY;
                        velocity.x = Mathf.Cos(groundAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);

                        collisionInfo.below = true; // is grounded
                        collisionInfo.slopeAngle = groundAngle;
                        collisionInfo.descending = true;

                    }
                }
            }
        }
    }

    



    


}
