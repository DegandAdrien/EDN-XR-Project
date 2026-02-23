using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPin : MonoBehaviour
{
    public float fallAngleThreshold = 45;
    public bool isFallen = false;
    public Transform quille;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 quilleDir = quille.position - transform.position;
        float angle = Vector3.Angle(quilleDir, transform.forward);

        if (angle < fallAngleThreshold)
        {
            isFallen = true;
        }
    }
   
}
