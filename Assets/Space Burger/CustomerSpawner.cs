using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] destinationPoints;

    [Header("Settings")]
    [SerializeField] private float minSpawnDelay = 5f;
    [SerializeField] private float maxSpawnDelay = 15f;

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
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            activeCustomers.RemoveAll(c => c == null);

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
