using UnityEngine;
using System.Collections;
using FOV;

namespace FOV
{
    public class Controller : MonoBehaviour
    {
        public float moveSpeed = 6;

        Rigidbody rigidBody;
        Camera viewCamera;
        Vector3 velocity;

        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            viewCamera = Camera.main;
        }

        void Update()
        {
            Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
            transform.LookAt(mousePos + Vector3.up * transform.position.y);
            velocity = new Vector3(Input.GetAxis("L_XAxis_1"), 0, Input.GetAxis("L_YAxis_1")).normalized * moveSpeed;
        }

        void FixedUpdate()
        {
            rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
        }
    }
}
