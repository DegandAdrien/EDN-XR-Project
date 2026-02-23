using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

public class FallingPin : MonoBehaviour
{
    public float fallAngleThreshold = 45f;

    public bool isFallen = false;

    void Update()
    {
        if (!isFallen)
        {
            float angle = Vector3.Angle(transform.up, Vector3.up);

            if (angle < fallAngleThreshold)
            {
                isFallen = true;
            }
        }
    }
}
