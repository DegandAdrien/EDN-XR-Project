using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlateReceiver : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 0.15f;
    [SerializeField] private Transform spawnPoint;

    private bool hasBurger;

    private void Update()
    {
        if (hasBurger || spawnPoint == null)
            return;

        var hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            var burger = FindBurgerRoot(hit);
            if (burger == null)
                continue;

            SnapBurger(burger);
            return;
        }
    }

    private void SnapBurger(GameObject burger)
    {
        ReleaseFromHand(burger);

        if (burger.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (burger.TryGetComponent(out XRGrabInteractable grab))
            grab.enabled = false;

        burger.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        burger.transform.SetParent(spawnPoint);

        hasBurger = true;
    }

    private static GameObject FindBurgerRoot(Collider other)
    {
        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.TryGetComponent(out Burger _))
        {
            return other.attachedRigidbody.gameObject;
        }

        var burgerInParent = other.GetComponentInParent<Burger>();
        return burgerInParent != null ? burgerInParent.gameObject : null;
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
