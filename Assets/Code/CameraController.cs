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
        centre.forward = (boxA.position - boxB.position).normalized;

        distance = Mathf.Clamp(Vector3.Distance(boxA.position, boxB.position), minDistance, 100);

        transform.position = centre.position - transform.forward * distance;
    }
}
