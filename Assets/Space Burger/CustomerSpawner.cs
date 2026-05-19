using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] destinationPoints;

    [Header("Délai de spawn (secondes)")]
    [SerializeField] private float spawnDelayAtStart = 20f;  // début de partie (facile)
    [SerializeField] private float spawnDelayAtEnd   = 4f;   // fin de partie (rush)

    private readonly bool[] occupiedPoints = new bool[5];
    private readonly List<Customer> activeCustomers = new();

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float progress = GameManager.Instance != null ? GameManager.Instance.GetGameProgress() : 0f;
            float delay = Mathf.Lerp(spawnDelayAtStart, spawnDelayAtEnd, progress);

            // Petite variation aléatoire de ±20% pour que ce ne soit pas trop mécanique
            delay *= Random.Range(0.8f, 1.2f);

            yield return new WaitForSeconds(delay);

            activeCustomers.RemoveAll(c => c == null);

            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                yield break;

            int freeIndex = GetFreeDestinationIndex();
            if (freeIndex != -1)
                SpawnCustomer(freeIndex);
        }
    }

    private void SpawnCustomer(int destinationIndex)
    {
        occupiedPoints[destinationIndex] = true;

        var go = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (go.TryGetComponent(out Customer customer))
        {
            customer.SetDestination(destinationPoints[destinationIndex]);
            customer.OnLeave += () => occupiedPoints[destinationIndex] = false;
            activeCustomers.Add(customer);
        }
    }

    private int GetFreeDestinationIndex()
    {
        for (int i = 0; i < destinationPoints.Length; i++)
        {
            if (!occupiedPoints[i])
                return i;
        }
        return -1;
    }
}
