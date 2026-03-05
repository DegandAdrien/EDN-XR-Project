using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class PanSteakReceiver : MonoBehaviour
{
    [SerializeField] private SteakCooker steakCooker;

    private void Reset()
    {
        var triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (steakCooker == null)
        {
            return;
        }

        var candidate = FindSteakRoot(other);
        if (candidate == null)
        {
            return;
        }

        steakCooker.TryPlaceSteak(candidate);
    }

    private static GameObject FindSteakRoot(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            if (other.attachedRigidbody.TryGetComponent(out CookedSteak cookedOnRb))
            {
                return null;
            }

            if (other.attachedRigidbody.TryGetComponent(out RawSteak rawOnRb))
            {
                return rawOnRb.gameObject;
            }

            if (other.attachedRigidbody.TryGetComponent(out XRGrabInteractable grabbableOnRb))
            {
                return grabbableOnRb.gameObject;
            }
        }

        if (other.TryGetComponent(out CookedSteak cookedOnCollider))
        {
            return null;
        }

        if (other.TryGetComponent(out RawSteak rawOnCollider))
        {
            return rawOnCollider.gameObject;
        }

        var cookedInParent = other.GetComponentInParent<CookedSteak>();
        if (cookedInParent != null)
        {
            return null;
        }

        var rawInParent = other.GetComponentInParent<RawSteak>();
        if (rawInParent != null)
        {
            return rawInParent.gameObject;
        }

        var grabbableInParent = other.GetComponentInParent<XRGrabInteractable>();
        return grabbableInParent != null ? grabbableInParent.gameObject : null;
    }
}
