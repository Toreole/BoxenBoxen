using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField]
    protected Transform target;
    
    void Update()
    {
        var tempPos = target.position;
        tempPos.y = transform.position.y;
        transform.forward = (tempPos - transform.position).normalized;
    }
}
