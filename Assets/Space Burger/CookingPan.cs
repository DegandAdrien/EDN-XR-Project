using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPan : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject rawSteakPrefab;
    public GameObject cookedSteakPrefab;

    [Header("Spawn Position")]
    public Transform spawnPoint; // Position où le steak sera placé

    [Header("Cooking Settings")]
    public float cookingTime = 5f;

    private GameObject currentSteak;
    private bool isCooking = false;

    public void PlaceRawSteak()
    {
        if (currentSteak != null) return;

        currentSteak = Instantiate(rawSteakPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        StartCoroutine(CookSteak());
    }

    IEnumerator CookSteak()
    {
        isCooking = true;

        yield return new WaitForSeconds(cookingTime);

        if (currentSteak != null)
        {
            Destroy(currentSteak);

            currentSteak = Instantiate(cookedSteakPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        }

        isCooking = false;
    }
}
