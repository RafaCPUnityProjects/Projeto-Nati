using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    new Rigidbody rigidbody;
    Vector3 velocity;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        velocity = new Vector3(Input.GetAxisRaw("L_XAxis_1"), 0, Input.GetAxisRaw("L_YAxis_1")).normalized * 10;
	}

    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
