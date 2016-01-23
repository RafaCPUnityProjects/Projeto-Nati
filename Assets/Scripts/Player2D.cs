using UnityEngine;
using System.Collections;

public class Player2D : MonoBehaviour
{
    new Rigidbody2D rigidbody;
    Vector2 velocity;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector2(Input.GetAxisRaw("L_XAxis_1"), Input.GetAxisRaw("L_YAxis_1")).normalized * 10;
    }

    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
