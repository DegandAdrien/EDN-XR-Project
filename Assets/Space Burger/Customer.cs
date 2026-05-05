using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private GameObject[] modelPrefabs;
    [SerializeField] private Transform modelSpawnPoint;

    [Header("Order")]
    [SerializeField] private int maxBurgers = 2;
    [SerializeField] private int maxFries = 2;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Patience")]
    [SerializeField] private float basePatienceTime = 20f;
    [SerializeField] private float timePerBurger = 10f;
    [SerializeField] private float timePerFries = 5f;
    [SerializeField] private Image patienceBarFill;

    private static readonly Color ColorGreen  = new Color(0.2f, 0.85f, 0.2f);
    private static readonly Color ColorOrange = new Color(1f, 0.55f, 0f);
    private static readonly Color ColorRed    = new Color(0.9f, 0.1f, 0.1f);

    [Header("Billboard")]
    [SerializeField] private Transform[] billboardTargets;

    [Header("Bubble")]
    [SerializeField] private TextMeshPro orderText;
    [SerializeField] private GameObject orderBubble;

    public event Action OnLeave;

    private int burgerCount;
    private int friesCount;
    private int initialBurgerCount;
    private int initialFriesCount;
    private float remainingPatience;
    private bool isSatisfied;
    private Transform destination;
    private bool isLeaving;
    private bool reachedDestination;

    private void SpawnRandomModel()
    {
        if (modelPrefabs == null || modelPrefabs.Length == 0) return;

        var randomModel = modelPrefabs[UnityEngine.Random.Range(0, modelPrefabs.Length)];
        var spawnParent = modelSpawnPoint != null ? modelSpawnPoint : transform;
        Instantiate(randomModel, spawnParent.position, spawnParent.rotation, spawnParent);
    }

    public void SetDestination(Transform dest)
    {
        destination = dest;
    }

    private void Start()
    {
        SpawnRandomModel();

        // Générer une commande aléatoire avec au moins un item
        burgerCount = UnityEngine.Random.Range(0, maxBurgers + 1);
        friesCount  = UnityEngine.Random.Range(0, maxFries + 1);

        if (burgerCount == 0 && friesCount == 0)
            burgerCount = 1;

        initialBurgerCount = burgerCount;
        initialFriesCount = friesCount;
        float patienceTime = basePatienceTime + burgerCount * timePerBurger + friesCount * timePerFries;
        remainingPatience = patienceTime;
        UpdateBubble();
        UpdatePatienceBar();

        if (billboardTargets != null)
            foreach (var t in billboardTargets)
                if (t != null) t.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (destination != null && !reachedDestination)
            MoveToDestination();

        if (!isLeaving && reachedDestination)
            UpdatePatience();
    }

    private void UpdateBillboard()
    {
        if (billboardTargets == null || Camera.main == null) return;

        foreach (var t in billboardTargets)
        {
            if (t == null) continue;
            t.gameObject.SetActive(true);
            Vector3 dir = t.position - Camera.main.transform.position;
            t.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }

    private void UpdatePatience()
    {
        remainingPatience -= Time.deltaTime;
        UpdatePatienceBar();

        if (remainingPatience <= 0f)
            Leave();
    }

    private void UpdatePatienceBar()
    {
        if (patienceBarFill == null) return;

        float totalPatience = basePatienceTime + initialBurgerCount * timePerBurger + initialFriesCount * timePerFries;
        float ratio = Mathf.Clamp01(remainingPatience / totalPatience);
        patienceBarFill.rectTransform.localScale = new Vector3(ratio, 1f, 1f);

        if (ratio > 0.5f)
            patienceBarFill.color = ColorGreen;
        else if (ratio > 0.25f)
            patienceBarFill.color = ColorOrange;
        else
            patienceBarFill.color = ColorRed;
    }

    private void MoveToDestination()
    {
        var target = new Vector3(destination.position.x, transform.position.y, destination.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        var direction = target - transform.position;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            reachedDestination = true;
            UpdateBillboard();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLeaving || !reachedDestination)
            return;

        // Livraison burger via assiette
        if (burgerCount > 0)
        {
            var plate = other.attachedRigidbody != null
                ? other.attachedRigidbody.GetComponent<PlateReceiver>()
                : other.GetComponentInParent<PlateReceiver>();

            if (plate != null && plate.HasBurger)
            {
                AcceptBurger(plate);
                return;
            }
        }

        // Livraison frites directement
        if (friesCount > 0)
        {
            var fries = other.attachedRigidbody != null
                ? other.attachedRigidbody.GetComponent<FrenchFries>()
                : other.GetComponentInParent<FrenchFries>();

            if (fries != null)
            {
                AcceptFries(fries);
                return;
            }
        }
    }

    private void AcceptBurger(PlateReceiver plate)
    {
        var burger = plate.GetComponentInChildren<Burger>();
        if (burger != null)
            Destroy(burger.gameObject);

        Destroy(plate.gameObject);

        burgerCount--;
        UpdateBubble();
        CheckIfSatisfied();
    }

    private void AcceptFries(FrenchFries fries)
    {
        Destroy(fries.gameObject);

        friesCount--;
        UpdateBubble();
        CheckIfSatisfied();
    }

    private void CheckIfSatisfied()
    {
        if (burgerCount <= 0 && friesCount <= 0)
        {
            isSatisfied = true;
            Leave();
        }
    }

    private void UpdateBubble()
    {
        if (orderText != null)
        {
            var text = "";
            if (burgerCount > 0)
                text += $"Burger x{burgerCount}";
            if (friesCount > 0)
            {
                if (text.Length > 0) text += "\n";
                text += $"Frites x{friesCount}";
            }
            orderText.text = text;
        }

        if (orderBubble != null)
            orderBubble.SetActive(reachedDestination && (burgerCount > 0 || friesCount > 0));
    }

    private void Leave()
    {
        isLeaving = true;

        if (orderBubble != null)
            orderBubble.SetActive(false);

        if (isSatisfied && ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(initialBurgerCount, initialFriesCount);

        OnLeave?.Invoke();
        Destroy(gameObject, 1f);
    }
}
