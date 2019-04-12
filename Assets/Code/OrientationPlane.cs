using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationPlane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var box in FindObjectsOfType<BoxController>())
            box.SetOrientationPlane(this.transform);
    }
}