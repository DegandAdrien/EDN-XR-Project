using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BowlingResetter : MonoBehaviour
{
   
    public InputActionReference resetAction;

    void Update()
    {
        if (resetAction != null && resetAction.action.triggered)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
