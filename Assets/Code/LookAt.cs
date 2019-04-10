using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField]
    protected Transform target;
    protected Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    //TODO: no hard look at.
    void Update()
    {
        var tempPos = target.position;
        tempPos.y = transform.position.y;
            
        var fwd = (tempPos - transform.position).normalized;
        //var tRot = Quaternion.Lerp( body.rotation, Quaternion.LookRotation(fwd, Vector3.up), 0.4f);
        var tRot = Quaternion.LookRotation(fwd, Vector3.up);
        body.MoveRotation(tRot);
        //transform.rotation = Quaternion.Lerp(transform.rotation, tRot, 0.1f);
        //body.AddTorque((-tRot + transform.rotation.eulerAngles ) * Time.deltaTime);
    }
}
