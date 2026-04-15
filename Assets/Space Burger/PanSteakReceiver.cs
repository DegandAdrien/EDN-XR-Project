using UnityEngine;

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
            if (other.attachedRigidbody.TryGetComponent(out RawSteak rawOnRb))
                return rawOnRb.gameObject;

            return null;
        }

        if (other.TryGetComponent(out RawSteak rawOnCollider))
            return rawOnCollider.gameObject;

        var rawInParent = other.GetComponentInParent<RawSteak>();
        return rawInParent != null ? rawInParent.gameObject : null;
    }
}
