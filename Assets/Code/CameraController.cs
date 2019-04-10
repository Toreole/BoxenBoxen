using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    protected float minDistance;

    [SerializeField]
    protected Transform boxA;
    [SerializeField]
    protected Transform boxB;

    [SerializeField]
    protected Transform centre;

    protected float distance;

    protected Vector3 MeanPosition => Vector3.Lerp(boxA.position, boxB.position, 0.5f);

    private void Start()
    {
        distance = Mathf.Clamp(Vector3.Distance(centre.position, transform.position), minDistance, 100);
    }

    //TODO: should be nicer, no more center shenanigans. better camera angle. follow a target at a distance.
    void Update()
    {
        centre.position = Vector3.Lerp(centre.position, MeanPosition, 0.05f);

        //smooth out rotation a little.
        var forward = (boxA.position - boxB.position);
        forward.y = 0;
        forward.Normalize();

        var tRot = Quaternion.LookRotation(forward, Vector3.up);
        centre.rotation = Quaternion.Lerp(centre.rotation, tRot, 0.1f);

        distance = Mathf.Clamp(Vector3.Distance(boxA.position, boxB.position), minDistance, Mathf.Infinity);

        transform.position = Vector3.Lerp(transform.position, centre.position - transform.forward * distance, 0.2f);
    }
}
