using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class CollisionInfo // object if something is near object == true;
    {
        public bool above, below;
        public bool left,right;

        public bool climbing; // re
        public float slopeAngle, slopeAngleOld;
        public bool descending;

        public int directionfacing; // represents side player is facing (means going left or right f.e) 1 == right -1 == left
        public bool fallingThrough; // represents value if player is falling through platform 

        // make something to remember touched layer for sliding/jumping in future to walls

        public Vector3 oldVelocity; // old velocity (used while descending or climbing)

        public void Reset() 
        {
            above = false;
            below = false;
            left = false;
            right = false;
            
            descending = false;
            climbing = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
