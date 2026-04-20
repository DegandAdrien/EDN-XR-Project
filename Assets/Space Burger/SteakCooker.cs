using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SteakCooker : MonoBehaviour
{
    [Header("Steak References")]
    [SerializeField] private GameObject currentSteak;
    [SerializeField] private GameObject cookedSteakPrefab;
    [SerializeField] private GameObject burnedSteakPrefab;

    [Header("Cooking")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float cookingTime = 5f;
    [SerializeField] private float burnTime = 4f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem cookedParticles;  // Vapeur quand le steak est cuit
    [SerializeField] private ParticleSystem burnedParticles;  // Flammes quand le steak brûle

    private Coroutine cookingRoutine;

    public bool HasCurrentSteak => currentSteak != null;

    public bool TryPlaceSteak(GameObject steak)
    {
        if (steak == null || HasCurrentSteak || spawnPoint == null
            || steak.GetComponent<CookedSteak>() != null
            || steak.GetComponent<BurnedSteak>() != null)
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

        // Nettoyer les particules d'un cycle précédent
        StopParticles(cookedParticles);
        StopParticles(burnedParticles);

        if (cookingRoutine == null)
        {
            cookingRoutine = StartCoroutine(CookSteakRoutine());
        }

        return true;
    }

    private IEnumerator CookSteakRoutine()
    {
        // Phase 1: raw → cooked
        yield return new WaitForSeconds(cookingTime);

        if (currentSteak == null || cookedSteakPrefab == null || spawnPoint == null)
        {
            cookingRoutine = null;
            yield break;
        }

        Destroy(currentSteak);
        var cookedSteak = Instantiate(cookedSteakPrefab, spawnPoint.position, spawnPoint.rotation);
        PrepareSteakForPickup(cookedSteak);
        currentSteak = cookedSteak;

        // Lancer la vapeur de cuisson
        PlayParticles(cookedParticles);

        if (cookedSteak.TryGetComponent(out XRGrabInteractable grab))
            grab.selectEntered.AddListener(_ => OnSteakPickedUp());

        // Phase 2: cooked → burned if not picked up in time
        yield return new WaitForSeconds(burnTime);

        if (currentSteak != null)
        {
            // Arrêter la vapeur, lancer les flammes
            StopParticles(cookedParticles);
            PlayParticles(burnedParticles);

            Destroy(currentSteak);
            currentSteak = null;

            if (burnedSteakPrefab != null && spawnPoint != null)
            {
                var burnedSteak = Instantiate(burnedSteakPrefab, spawnPoint.position, spawnPoint.rotation);
                PrepareSteakForPickup(burnedSteak);
            }
        }

        cookingRoutine = null;
    }

    private void OnSteakPickedUp()
    {
        currentSteak = null;
        StopParticles(cookedParticles);
        StopParticles(burnedParticles);
    }

    private static void PlayParticles(ParticleSystem ps)
    {
        if (ps == null) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();
    }

    private static void StopParticles(ParticleSystem ps)
    {
        if (ps == null) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private static void PrepareSteakForPickup(GameObject steak)
    {
        if (steak == null)
            return;

        steak.transform.SetParent(null);

        if (steak.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (steak.TryGetComponent(out XRGrabInteractable grabInteractable))
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
