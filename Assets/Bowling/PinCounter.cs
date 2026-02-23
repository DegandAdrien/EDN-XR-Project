using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class PinCounter : MonoBehaviour
{
    private FallingPin[] pins;
    public TextMeshProUGUI scoreText;

    private int fallenCount = 0;

    void Start()
    {
        
    }

    void Update()
    {
        pins = GetComponentsInChildren<FallingPin>();

        fallenCount = 0;

        foreach (FallingPin pin in pins)
        {
            if (pin.isFallen)
            {
                fallenCount++;
            }
        }

        scoreText.text = fallenCount.ToString();
    }
}
