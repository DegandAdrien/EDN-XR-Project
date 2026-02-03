using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject ball;
    public GameObject pin;

    void OnCollisionEnter(Collision otherObj)
    {
        Instantiate(ball, new Vector3(0.09, 0.3, 0.45), Quaternion.identity);

        Instantiate(pin, new Vector3(0, 0.5, 0.8), Quaternion.identity);
        Instantiate(pin, new Vector3(0.188, 0.5, 1.048), Quaternion.identity);
        Instantiate(pin, new Vector3(-0.22, 0.5, 1.051), Quaternion.identity);
        Instantiate(pin, new Vector3(0, 0.5, 1.3), Quaternion.identity);
        Instantiate(pin, new Vector3(0.4, 0.5, 1.3), Quaternion.identity);
        Instantiate(pin, new Vector3(-0.4, 0.5, 1.3), Quaternion.identity);
        Instantiate(pin, new Vector3(-0.2, 0.5, 1.5), Quaternion.identity);
        Instantiate(pin, new Vector3(0.2, 0.5, 1.5), Quaternion.identity);
        Instantiate(pin, new Vector3(0.6, 0.5, 1.5), Quaternion.identity);
        Instantiate(pin, new Vector3(-0.7, 0.5, 1.5), Quaternion.identity);
    }
}
