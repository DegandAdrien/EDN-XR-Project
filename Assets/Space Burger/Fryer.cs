using System.Collections;
using UnityEngine;

public class Fryer : MonoBehaviour
{
    [System.Serializable]
    public class BasketSlot
    {
        public Transform basket;          // Le panier du modèle
        public Transform potatoPosition;  // Où la patate apparaît dans ce panier
        public Transform friesSpawnPoint; // Où les frites sortent pour ce panier

        [HideInInspector] public bool isFrying = false;
        [HideInInspector] public GameObject currentItem;
        [HideInInspector] public Vector3 defaultLocalPos;
    }

    [Header("Prefabs")]
    public GameObject potatoPrefab;
    public GameObject frenchFriesPrefab;

    [Header("Baskets")]
    public BasketSlot[] baskets;

    [Header("Basket Animation")]
    public float basketLoweredOffset = -0.15f;
    public float basketAnimSpeed = 2f;

    [Header("Frying Settings")]
    public float fryingTime = 6f;

    private void Start()
    {
        foreach (var slot in baskets)
        {
            if (slot.basket != null)
                slot.defaultLocalPos = slot.basket.localPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Potato potato = other.GetComponentInParent<Potato>();
        if (potato == null) return;

        // Chercher le premier panier libre
        foreach (var slot in baskets)
        {
            if (!slot.isFrying)
            {
                Destroy(potato.gameObject);
                StartCoroutine(FryPotato(slot));
                return;
            }
        }
    }

    IEnumerator FryPotato(BasketSlot slot)
    {
        slot.isFrying = true;

        if (potatoPrefab != null && slot.potatoPosition != null)
        {
            slot.currentItem = Instantiate(potatoPrefab, slot.potatoPosition.position, slot.potatoPosition.rotation, slot.basket);

            Rigidbody rb = slot.currentItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider col = slot.currentItem.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
        }

        // Descendre le panier
        yield return StartCoroutine(MoveBasket(slot, slot.defaultLocalPos, slot.defaultLocalPos + Vector3.up * basketLoweredOffset));

        yield return new WaitForSeconds(fryingTime);

        if (slot.currentItem != null)
            Destroy(slot.currentItem);

        // Remonter le panier
        yield return StartCoroutine(MoveBasket(slot, slot.basket.localPosition, slot.defaultLocalPos));

        if (frenchFriesPrefab != null && slot.friesSpawnPoint != null)
            Instantiate(frenchFriesPrefab, slot.friesSpawnPoint.position, slot.friesSpawnPoint.rotation);

        slot.isFrying = false;
    }

    IEnumerator MoveBasket(BasketSlot slot, Vector3 from, Vector3 to)
    {
        if (slot.basket == null) yield break;

        float elapsed = 0f;
        float duration = 1f / basketAnimSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slot.basket.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        slot.basket.localPosition = to;
    }
}
