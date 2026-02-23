using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject ball;
    public GameObject pin;
    public GameObject spawnPlace;

    public void generateBowling()
    {
        Instantiate(ball, new Vector3(0.09f, 0.3f, 0.45f), Quaternion.identity);

        GameObject pin1 = Instantiate(pin, new Vector3(0f, 0.5f, 0.8f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin2 = Instantiate(pin, new Vector3(0.2f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin3 = Instantiate(pin, new Vector3(-0.2f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin4 = Instantiate(pin, new Vector3(0f, 0.5f, 1.3f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin5 = Instantiate(pin, new Vector3(0.4f, 0.5f, 1.3f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin6 = Instantiate(pin, new Vector3(-0.4f, 0.5f, 1.3f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin7 = Instantiate(pin, new Vector3(-0.2f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin8 = Instantiate(pin, new Vector3(0.2f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin9 = Instantiate(pin, new Vector3(0.6f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));
        GameObject pin10 = Instantiate(pin, new Vector3(-0.7f, 0.5f, 1.5f), Quaternion.Euler(-90f, 0f, 0f));

        pin1.transform.parent = spawnPlace.transform;
        pin2.transform.parent = spawnPlace.transform;
        pin3.transform.parent = spawnPlace.transform;
        pin4.transform.parent = spawnPlace.transform;
        pin5.transform.parent = spawnPlace.transform;
        pin6.transform.parent = spawnPlace.transform;
        pin7.transform.parent = spawnPlace.transform;
        pin8.transform.parent = spawnPlace.transform;
        pin9.transform.parent = spawnPlace.transform;
        pin10.transform.parent = spawnPlace.transform;
    }
}
