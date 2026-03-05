using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SteakCooker : MonoBehaviour
{
    [Header("Steak References")]
    [SerializeField] private GameObject currentSteak;
    [SerializeField] private GameObject cookedSteakPrefab;

    [Header("Cooking")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float cookingTime = 5f;

    private Coroutine cookingRoutine;

    public bool HasCurrentSteak => currentSteak != null;

    public bool TryPlaceSteak(GameObject steak)
    {
        if (steak == null || HasCurrentSteak || spawnPoint == null || steak.GetComponent<CookedSteak>() != null)
        {
            return false;
        }

        ReleaseFromHandIfNeeded(steak);

        currentSteak = steak;
        currentSteak.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        currentSteak.transform.SetParent(spawnPoint);

        if (currentSteak.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (currentSteak.TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            grabInteractable.enabled = false;
        }

        if (cookingRoutine == null)
        {
            cookingRoutine = StartCoroutine(CookSteakRoutine());
        }

        return true;
    }

    private IEnumerator CookSteakRoutine()
    {
        yield return new WaitForSeconds(cookingTime);

        if (currentSteak != null && cookedSteakPrefab != null && spawnPoint != null)
        {
            Destroy(currentSteak);
            var cookedSteak = Instantiate(cookedSteakPrefab, spawnPoint.position, spawnPoint.rotation);
            PrepareCookedSteakForPickup(cookedSteak);
            currentSteak = null;
        }

        cookingRoutine = null;
    }

    private static void PrepareCookedSteakForPickup(GameObject cookedSteak)
    {
        if (cookedSteak == null)
        {
            return;
        }

        cookedSteak.transform.SetParent(null);

        if (cookedSteak.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (cookedSteak.TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            grabInteractable.enabled = true;
        }
    }

    private void ReleaseFromHandIfNeeded(GameObject steak)
    {
        if (!steak.TryGetComponent(out XRGrabInteractable grabInteractable) || !grabInteractable.isSelected)
        {
            return;
        }

        var interactor = grabInteractable.interactorsSelecting.Count > 0
            ? grabInteractable.interactorsSelecting[0]
            : null;

        if (interactor == null || grabInteractable.interactionManager == null)
        {
            return;
        }

        grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);
    }
}
