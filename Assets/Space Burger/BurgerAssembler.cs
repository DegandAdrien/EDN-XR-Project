using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BurgerAssembler : MonoBehaviour
{
    [SerializeField] private GameObject burgerPrefab;
    [SerializeField] private float detectionRadius = 0.15f;

    private void Update()
    {
        if (burgerPrefab == null)
            return;

        var hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            var steak = FindCookedSteakRoot(hit);
            if (steak == null)
                continue;

            var spawnPosition = transform.position;

            ReleaseFromHand(steak);
            ReleaseFromHand(gameObject);

            Destroy(steak);
            Destroy(gameObject);

            Instantiate(burgerPrefab, spawnPosition, Quaternion.identity);
            return;
        }
    }

    private static GameObject FindCookedSteakRoot(Collider other)
    {
        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.TryGetComponent(out CookedSteak _))
        {
            return other.attachedRigidbody.gameObject;
        }

        var cookedInParent = other.GetComponentInParent<CookedSteak>();
        return cookedInParent != null ? cookedInParent.gameObject : null;
    }

    private static void ReleaseFromHand(GameObject obj)
    {
        if (!obj.TryGetComponent(out XRGrabInteractable grab) || !grab.isSelected)
            return;

        var interactor = grab.interactorsSelecting.Count > 0
            ? grab.interactorsSelecting[0]
            : null;

        if (interactor == null || grab.interactionManager == null)
            return;

        grab.interactionManager.SelectExit(interactor, grab);
    }
}
