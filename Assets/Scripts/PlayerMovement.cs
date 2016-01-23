using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public string playerNumber = "1";
    public float moveSpeed = 0.1f;
    public float rotateSpeed = 2.0f;
    public float jumpForce = 10.0f;
    public float isGroundedSize = 1f;

    Rigidbody myRigidBody;
    string xAxisName;
    string yAxisName;
    string jumpButton;
    string grabButton;
    string hitButton;
    bool jumping = false;


    // Use this for initialization
    void Start()
    {
        xAxisName = "L_XAxis_" + playerNumber;
        yAxisName = "L_YAxis_" + playerNumber;
        jumpButton = "A_" + playerNumber;
        grabButton = "X_" + playerNumber;
        hitButton = "B_" + playerNumber;

        myRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //string xAxisName = "L_XAxis_" + playerNumber;
        //string yAxisName = "L_YAxis_" + playerNumber;
        //string jumpButton = "A_" + playerNumber;
        //string grabButton = "X_" + playerNumber;
        //string hitButton = "B_" + playerNumber;

        float xAxisValue = Input.GetAxisRaw(xAxisName);
        float yAxisValue = Input.GetAxisRaw(yAxisName);

        //transform.Rotate(Vector3.up, xAxisValue * rotateSpeed);

        float jump = 0.0f;

        if (Input.GetButtonDown(jumpButton) && IsGrounded())
        {
            jumping = true;
            jump = jumpForce;
        }

        //float speed = yAxisValue * moveSpeed * Time.fixedDeltaTime;
        Vector3 movementVector = new Vector3(xAxisValue, 0, yAxisValue).normalized * moveSpeed;

        //Vector3 forwardVector = transform.TransformDirection(Vector3.forward);

        //movementVector = new Vector3(forwardVector.x * movementVector.x, forwardVector.y * movementVector.y, forwardVector.z * movementVector.z);

        //myRigidBody.AddRelativeForce(movementVector);
        myRigidBody.MovePosition(myRigidBody.position + movementVector * Time.fixedDeltaTime);
    }

    bool IsGrounded()
    {
        Vector3 down = transform.TransformDirection(Vector3.down);
        return Physics.Raycast(transform.position, down, isGroundedSize);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 direction = transform.TransformDirection(Vector3.down) * isGroundedSize;
        Gizmos.DrawRay(transform.position, direction);
    }
}
