using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public struct FocusArea // need to fix bugging while on moving platform
    {
        public Vector2 Center;
        public Vector2 velocity;
        public float left;
        public float right;
        public float top;
        public float bottom;

        public FocusArea(Bounds targetBounds, Vector2 sizeOfArea) // area in which player can move, but once reaches bound of it, it moves camera a little so bound is being moved too
        {
            left = targetBounds.center.x - sizeOfArea.x / 2;
            right = targetBounds.center.x + sizeOfArea.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + sizeOfArea.y;

            velocity = Vector2.zero;
            Center = new Vector2((left + right) / 2, (top + bottom) / 2); // getting center point by dividing both oposite sides after summing them up
        }

        public void Update(Bounds bounds)
        {
            float targetX = 0; // checks if player is moving against bounds from left or right
            if (bounds.min.x < left)
            {
                targetX = bounds.min.x - left;
            }
            else if (bounds.max.x > right)
            {
                targetX = bounds.max.x - right;
            }
            left += targetX;
            right += targetX;

            float targetY = 0; // checks if player is moving against bounds from top or bottom 
            if (bounds.min.y < bottom)
            {
                targetY = bounds.min.y - bottom;
            }
            else if (bounds.max.y > top)
            {
                targetY = bounds.max.y - top;
            }
            top += targetY;
            bottom += targetY;
            velocity = new Vector2(targetX, targetY);
            Center = new Vector2((left + right) / 2, (top + bottom) / 2); // updating center point position

        }



    }
}
