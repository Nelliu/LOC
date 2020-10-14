using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller))]  
public class Player : MonoBehaviour
{
    public LayerMask Wall; // needs work around

    public float MaxJumpHeight = 1.3f;
    public float MinJumpHeight = 0.5f;
    public float TimeToGetToHeight = 0.4f; // time it takes to reach jump height
    private float accelerationTimeOnGroud = .1f;
    private float accelerationTimeInSky = .05f;

    public float movementSpeed = 6;

    public Sprite sprite1; // sprite for cube
    public Sprite sprite2; // sprite for square

    public float Gravity; // represents gravity after calculations
    public float MaxJumpVelocity;
    public float MinJumpVelocity;

    private Vector3 Velocity;
    private float NonJumpingVelocityMax; // represents value once player doesn't jump anymore and is just falling 
    private float velSmooth;

    private bool WSlide = false; // stores if player is sliding wall down or not
    public float WSSpeed = 1.5f; // stores speed of sliding on wall
    private float wStickTime = .05f; // character sticks that long to wall before starts falling 
    private float unstickFromWall; // time we have to jump before character starts falling off wall

    private Controller controller; // ok
    public GameObject EscMenu; // represents game object of Escape menu

    public Vector2 wallJumpUp; // represents force of player when he jumps up on wall
    public Vector2 wallJumpDown; // represents how far from wall will player go if he just wants to fall down from it
    public Vector2 wallJumpLeap; // represents force how far will player jump while leaping between two walls
    private int wallDirectionX;

    


    void Start()
    {
        NonJumpingVelocityMax = MaxJumpHeight;
        controller = GetComponent<Controller>();


        Gravity = -(2 * MaxJumpHeight) / Mathf.Pow(TimeToGetToHeight, 2);
        MaxJumpVelocity = Mathf.Abs(Gravity) * TimeToGetToHeight;
        MinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Gravity) * MinJumpHeight);
        
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // recognizes player input


        if (controller.collisionInfo.left) { wallDirectionX = -1; } else { wallDirectionX = 1; }

        float FinVelocityX = input.x * movementSpeed; // represents max movement speed
        Velocity.x = Mathf.SmoothDamp(Velocity.x, FinVelocityX, ref velSmooth, (controller.collisionInfo.below) ? accelerationTimeOnGroud : accelerationTimeInSky); // accelerates object

        WSlide = false;
        if ((controller.collisionInfo.left || controller.collisionInfo.right) && !controller.collisionInfo.below && Velocity.y < 0 )
        {
            WSlide = true;

            if (Velocity.y < -WSSpeed)
            {
                Velocity.y = -WSSpeed;
            }
            if (unstickFromWall > 0)
            {
                Velocity.x = 0;
                velSmooth = 0;

                if (input.x != wallDirectionX && input.x != 0)
                {
                    unstickFromWall -= Time.deltaTime;
                }
                else
                {
                    unstickFromWall = wStickTime;
                }
                
            }
            else
            {
                unstickFromWall = wStickTime;
            }

        }

        if (Input.GetKeyDown(KeyCode.W)) //&& jumpCount != 2) //&& controller.collisionInfo.below)
        {

            if (WSlide)
            {
                if (wallDirectionX == input.x)
                {
                    Velocity.x = -wallDirectionX * wallJumpUp.x;
                    Velocity.y = wallJumpUp.y;
                }
                else if (input.x == 0)
                {
                    Velocity.x = -wallDirectionX * wallJumpDown.x;
                    Velocity.y = wallJumpDown.y;
                }
                else
                {
                    Velocity.x = -wallDirectionX * wallJumpLeap.x;
                    Velocity.y = wallJumpLeap.y;
                }
            }
            if (controller.collisionInfo.below)
            {
                Velocity.y = MaxJumpVelocity;
            }

        }
       if (Input.GetKeyUp(KeyCode.W))
       {
            if (Velocity.y > MinJumpVelocity)
            {
                Velocity.y = MinJumpVelocity;
            }
            
       }
        Velocity.y += Gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime, input);

        if (controller.collisionInfo.above || controller.collisionInfo.below)
        {
            Velocity.y = 0; // hold onto wall
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isSquare)
        {
            if (float.IsNaN(Gravity))
            {
                Gravity = 9.9f;
            }
            else if (Gravity > 0)
            {
                Gravity = -9.9f;
            }
            else if (Gravity < 0)
            {
                Gravity = 9.9f;
            }
           
           // Gravity = (float.IsNaN(Gravity)) ? 1f : -9.9f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (!EscMenu.activeSelf)
            {
                EscMenu.SetActive(true);
            }
            else EscMenu.SetActive(false);

        }
    }

    //private void UpdateStartPoints()
    //{
    //    if (controller.nameOfCheckpoint == "Finish1" || controller.nameOfCheckpoint == "SqFinish")
    //    {
    //        if (controller.nameOfCheckpoint == "Finish1")
    //        {
    //            Invoke
    //        }
    //    }
    //}

   

}
