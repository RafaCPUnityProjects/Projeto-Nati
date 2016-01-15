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
        if(controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0.0f;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("L_XAxis_" + playerNumber), Input.GetAxisRaw("L_YAxis_" + playerNumber));
        if (Input.GetButtonDown("A_" + playerNumber) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
        float targetVelocity = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing, controller.collisions.below?accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
