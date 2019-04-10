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

    // Update is called once per frame
    void Update()
    {
        centre.position = MeanPosition;

        //smooth out rotation a little.
        var forward = (boxA.position - boxB.position).normalized;
        var tRot = Quaternion.LookRotation(forward, Vector3.up);
        centre.rotation = Quaternion.Lerp(centre.rotation, tRot, 0.2f);

        distance = Mathf.Clamp(Vector3.Distance(boxA.position, boxB.position), minDistance, 100);

        transform.position = centre.position - transform.forward * distance;
    }
}
