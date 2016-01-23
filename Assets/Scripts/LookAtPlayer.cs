using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {
    public Transform target;

    Vector3 originalPos;
	// Use this for initialization
	void Start () {
        originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = originalPos + target.position;
	}
}
