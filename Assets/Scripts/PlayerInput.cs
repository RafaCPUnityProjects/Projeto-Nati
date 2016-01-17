using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    public string playerNumber = "1";
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    private float gravity;
    private float jumpVelocity;
    private float moveSpeed = 6;

    private Vector3 velocity;
    private float velocityXSmoothing;

    private PlayerController controller;


    void Start()
    {
        controller = GetComponent<PlayerController>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        print("Gravity: " + gravity + " JumpVel: " + jumpVelocity);
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("L_XAxis_" + playerNumber), Input.GetAxisRaw("L_YAxis_" + playerNumber));
        int wallDirX = controller.collisions.left ? -1 : 1;

        float targetVelocity = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);

        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                print("Sticking to wall");
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0.0f;
        }
        if (Input.GetButtonDown("A_" + playerNumber))
        {
            if (wallSliding)
            {
                print("input x = " + input.x);

                if (wallDirX == input.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (input.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if (controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
