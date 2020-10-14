using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Raycast : MonoBehaviour
{

    public LayerMask CollisionMask;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;


    public const float skinWidth = .015f;
    public int horizontalRayCount = 5;
    public int verticalRayCount = 5;


    public struct RaycastOrigins // raycast points in square (like points from where does ray start coming from)
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
        public float slopeAngle, slopeAngleOld;

    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        //Physics2D.SyncTransforms();

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);


        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }



    public virtual void Awake() // this method has to be awake instead of Start because of collider getting component is must to be first 
    {

        collider = GetComponent<BoxCollider2D>();
        
    }
    public virtual void Start()
    {
        CalculateRaySpacing();
    }


}
