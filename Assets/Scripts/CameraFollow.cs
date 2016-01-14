using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform[] targets;
    Vector3 targetPos;
    Vector3 startPos;
    // Use this for initialization
    void Start()
    {


        startPos = transform.position;
        //transform.LookAt(targets);
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = Vector3.zero;

        foreach (Transform target in targets)
        {
            targetPos += target.position;
        }
        targetPos = targetPos / targets.Length;

        transform.position = new Vector3(targetPos.x, targetPos.y, startPos.z);
    }
}
