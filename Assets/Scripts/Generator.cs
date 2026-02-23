using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject ball;
    public GameObject pin;

    public void generateBowling()
    {
        Instantiate(ball, new Vector3(0.09f, 0.3f, 0.45f), Quaternion.identity);

        Instantiate(pin, new Vector3(0f, 0.5f, 0.8f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(0.2, 0.5f, 1.5), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(-0.2f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(0f, 0.5f, 1.3f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(0.4f, 0.5f, 1.3f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(-0.4f, 0.5f, 1.3f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(-0.2f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(0.2f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(0.6f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(pin, new Vector3(-0.7f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
    }
}
