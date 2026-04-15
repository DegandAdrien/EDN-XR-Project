using System;
using TMPro;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("Order")]
    [SerializeField] private int burgerCount = 1;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Bubble")]
    [SerializeField] private TextMeshPro orderText;
    [SerializeField] private GameObject orderBubble;

    public event Action OnLeave;

    private Transform destination;
    private bool isLeaving;
    private bool reachedDestination;

    public void SetDestination(Transform dest)
    {
        destination = dest;
    }

    private void Start()
    {
        UpdateBubble();
    }

    private void Update()
    {
        if (destination != null && !reachedDestination)
            MoveToDestination();
    }

    private void MoveToDestination()
    {
        var target = new Vector3(destination.position.x, transform.position.y, destination.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        var direction = target - transform.position;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, target) < 0.05f)
            reachedDestination = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLeaving || !reachedDestination)
            return;

        var plate = other.attachedRigidbody != null
            ? other.attachedRigidbody.GetComponent<PlateReceiver>()
            : other.GetComponentInParent<PlateReceiver>();

        if (plate == null || !plate.HasBurger)
            return;

        AcceptDelivery(plate);
    }

    private void AcceptDelivery(PlateReceiver plate)
    {
        var burger = plate.GetComponentInChildren<Burger>();
        if (burger != null)
            Destroy(burger.gameObject);

        Destroy(plate.gameObject);

        burgerCount--;
        UpdateBubble();

        if (burgerCount <= 0)
            Leave();
    }

    private void UpdateBubble()
    {
        if (orderText != null)
            orderText.text = $"Burger x{burgerCount}";

        if (orderBubble != null)
            orderBubble.SetActive(burgerCount > 0);
    }

    private void Leave()
    {
        isLeaving = true;

        if (orderBubble != null)
            orderBubble.SetActive(false);

        OnLeave?.Invoke();
        Destroy(gameObject, 1f);
    }
}
