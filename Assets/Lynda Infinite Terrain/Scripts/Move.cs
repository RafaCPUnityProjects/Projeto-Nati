using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    public float speed = 1;
    public float fastSpeedMultiplier = 3;
    
    void Start()
    {

    }

    void Update()
    {
        var currentSpeed = speed;
        var movement = new Vector3(Input.GetAxis("L_XAxis_1"), Input.GetAxis("L_YAxis_1"));
        if(Input.GetButton("A_1"))
        {
            currentSpeed = fastSpeedMultiplier * speed;
        }
        transform.Translate(movement * currentSpeed * Time.deltaTime);
    }
}
