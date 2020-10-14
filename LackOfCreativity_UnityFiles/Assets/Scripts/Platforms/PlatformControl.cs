using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts.Platforms
{

    public class PlatformControl : Raycast
    {


        public LayerMask passengerMask;
        public Vector3 Move; // move without waypoints, manually
        private List<PlayerPlatformMove> movementOnPlatform = new List<PlayerPlatformMove>();

        public Vector3[] Waypoints; // list of waypoints where platform should stop - local positions relative to object
        private Vector3[] GWaypoints; // represents waipoints global position

        public float PlatformSpeed;
        private int fromBeforeWaypointI; // index of previous waypoint
        private float percentage; // percentage of where we are in between waypoints -- 0.5 means we are half way to another waypoint


        public override void Start()
        {
            base.Start();


            GWaypoints = new Vector3[Waypoints.Length];
            for (int i = 0; i < Waypoints.Length; i++)
            {
                GWaypoints[i] = Waypoints[i] + transform.position;
            }

        }


        void Update()
        {
            UpdateRaycastOrigins();
            Vector3 velocity = MoveBetweenWaypoints();//* Time.deltaTime;

            MoveObjectPassanger(velocity);

            MoveObjectsOnPlatform(true);
            transform.Translate(velocity);
            MoveObjectsOnPlatform(false);

         
        }
        
        private Vector3 MoveBetweenWaypoints()
        {
            int headingToWaypointI = fromBeforeWaypointI + 1; // current waypoint we are heading to
            float distanceBetween = Vector3.Distance(GWaypoints[fromBeforeWaypointI], GWaypoints[headingToWaypointI]);
           
            percentage += Time.deltaTime * PlatformSpeed/distanceBetween;
       
           
            Vector3 newPosition = Vector3.Lerp(GWaypoints[fromBeforeWaypointI], GWaypoints[headingToWaypointI], percentage);
            
            if (percentage >= 1)
            {
                percentage = 0;
                fromBeforeWaypointI++;
                
                if (fromBeforeWaypointI >= GWaypoints.Length - 1)
                {
                    fromBeforeWaypointI = 0;
                    Array.Reverse(GWaypoints);
                }
            }
          
            return newPosition - transform.position;

        }

        private void MoveObjectsOnPlatform(bool moveBeforePlatform)
        {
            for (int i = 0; i < movementOnPlatform.Count; i++)
            {
                if (movementOnPlatform[i].BeforePlatform == moveBeforePlatform)
                {
                    movementOnPlatform[i].transform.GetComponent<Controller>().Move(movementOnPlatform[i].velocity, movementOnPlatform[i].OnPlatform);
                }
            }
        }

        private void MoveObjectPassanger(Vector3 velocity) // function which makes player move if hit by object
        {
            HashSet<Transform> AllmovedPassengers = new HashSet<Transform>(); // in case more objects travel at same time
            movementOnPlatform = new List<PlayerPlatformMove>();
            float dirX = Mathf.Sign(velocity.x);
            float dirY = Mathf.Sign(velocity.y);

            // for vert moving
            if (velocity.y != 0)
            {
                float rayLength = Mathf.Abs(velocity.y) + skinWidth;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                    rayOrigin += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                       
                        if (!AllmovedPassengers.Contains(hit.transform))
                        {
                            AllmovedPassengers.Add(hit.transform);
                            float pushX = (dirY == 1) ? velocity.x : 0;
                            float pushY = velocity.y - (hit.distance - skinWidth) * dirY;
                            

                            bool movingUp = dirY == 1; // if moving up == true
                            //print(movingUp); // is on platform that can move
                            movementOnPlatform.Add(new PlayerPlatformMove(hit.transform, new Vector3(pushX, pushY), movingUp, true));


                        }


                    }
                }


            }
            if (velocity.x != 0)
            {
                float rayLength = Mathf.Abs(velocity.x) + skinWidth;

                for (int i = 0; i < horizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                    rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!AllmovedPassengers.Contains(hit.transform))
                        {
                            AllmovedPassengers.Add(hit.transform);
                            float pushX = velocity.x - (hit.distance - skinWidth) * dirX;
                            float pushY = -skinWidth; // can't be 0, player can't jump if being pushed by platform otherwise
                          


                            
                            movementOnPlatform.Add(new PlayerPlatformMove(hit.transform, new Vector3(pushX, pushY), false, true));
                        }
                    }
                }
            }

            // if player is on top  horizontally or down moving platform, he moves along with it 
            if (dirY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                float rayLength = skinWidth * 2;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!AllmovedPassengers.Contains(hit.transform))
                        {
                            AllmovedPassengers.Add(hit.transform);
                            float pushX = velocity.x;
                            float pushY = velocity.y;

                            movementOnPlatform.Add(new PlayerPlatformMove(hit.transform, new Vector3(pushX, pushY), true, false));
                        }
                    }
                }
            }





        }

        private void OnDrawGizmos()
        {
            if (Waypoints != null)
            {
                Gizmos.color = Color.yellow;
                float size = 0.1f;

                for (int i = 0; i < Waypoints.Length; i++)
                {
                    Vector3 waypointGlobal = (Application.isPlaying)? GWaypoints[i] : Waypoints[i] + gameObject.transform.position;
                    //Gizmos.DrawLine(waypointGlobal, gameObject.transform.position );
                    Gizmos.DrawLine(waypointGlobal - Vector3.up * size, waypointGlobal + Vector3.up * size);
                    Gizmos.DrawLine(waypointGlobal - Vector3.left * size, waypointGlobal + Vector3.left * size);
                }

            }
           
        }

    }
}
