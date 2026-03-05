using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class RawSteakDispenser : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject rawSteakPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnCooldown = 0.35f;

    [Header("Right Hand")]
    [SerializeField] private XRRayInteractor rightRayInteractor;

    private XRBaseInteractable interactable;
    private bool wasPressedLastFrame;
    private float nextSpawnTime;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }

    private void Update()
    {
        if (rawSteakPrefab == null || spawnPoint == null || rightRayInteractor == null || interactable == null)
        {
            wasPressedLastFrame = false;
            return;
        }

        var isHoveringThisDispenser = rightRayInteractor.IsHovering(interactable);
        var isPressedNow = isHoveringThisDispenser && IsRightTriggerPressed();

        if (isPressedNow && !wasPressedLastFrame && Time.time >= nextSpawnTime)
        {
            SpawnRawSteak();
            nextSpawnTime = Time.time + spawnCooldown;
        }

        wasPressedLastFrame = isPressedNow;
    }

    private bool IsRightTriggerPressed()
    {
        var controller = rightRayInteractor.xrController;
        if (controller == null)
        {
            return false;
        }

        return controller.activateInteractionState.active;
    }

    private void SpawnRawSteak()
    {
        Instantiate(rawSteakPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
