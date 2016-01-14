using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    Vector3 relativeStartPos;
    // Use this for initialization
    void Start()
    {
        relativeStartPos = target.position - transform.position;
        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position - relativeStartPos;
    }
}
