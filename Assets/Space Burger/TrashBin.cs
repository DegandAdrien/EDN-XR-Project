using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class TrashBin : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var root = FindGrabbableRoot(other);
        if (root != null)
            Destroy(root);
    }

    private static GameObject FindGrabbableRoot(Collider other)
    {
        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.TryGetComponent(out XRGrabInteractable _))
        {
            return other.attachedRigidbody.gameObject;
        }

        var grabbableInParent = other.GetComponentInParent<XRGrabInteractable>();
        return grabbableInParent != null ? grabbableInParent.gameObject : null;
    }
}
