using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private EquipmentController equipmentController;

    [SerializeField]
    private Camera playerCamera;

    [Header("Interaction")]
    [SerializeField]
    private float range = 3f;

    [SerializeField]
    private KeyCode interactKey = KeyCode.E;

    [Header("Combat")]
    [SerializeField]
    private float hitCooldown = 0.35f;

    [SerializeField]
    private int gatherDamage = 1;

    [SerializeField]
    private int attackDamage = 1;

    private float nextHitTime;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (equipmentController == null)
        {
            equipmentController =
                GetComponent<EquipmentController>();
        }

        if (playerCamera == null)
        {
            Debug.LogError(
                "PlayerInteraction: Player Camera was not found."
            );
        }

        if (equipmentController == null)
        {
            Debug.LogError(
                "PlayerInteraction: EquipmentController was not found."
            );
        }
    }

    private void Update()
    {
        // Δεν εκτελούμε interactions όταν κάποιο UI είναι ανοικτό.
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryHit();
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        bool foundTarget = Physics.Raycast(
            ray,
            out RaycastHit hit,
            range,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        if (!foundTarget)
        {
            return;
        }

        IInteractable interactable =
            hit.collider.GetComponentInParent<IInteractable>();

        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    private void TryHit()
    {
        if (playerCamera == null)
        {
            return;
        }

        // Όταν κρατάμε Workbench ή άλλο building,
        // το αριστερό κλικ χρησιμοποιείται μόνο για placement.
        ItemData equippedItem =
            equipmentController != null
                ? equipmentController.CurrentItem
                : null;

        if (equippedItem != null &&
            equippedItem.isPlaceable)
        {
            return;
        }

        if (Time.time < nextHitTime)
        {
            return;
        }

        nextHitTime = Time.time + hitCooldown;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        bool foundTarget = Physics.Raycast(
            ray,
            out RaycastHit hit,
            range,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        if (!foundTarget)
        {
            return;
        }

        ResourceNode resource =
            hit.collider.GetComponentInParent<ResourceNode>();

        if (resource != null)
        {
            int damage = GetResourceDamage(resource);

            resource.Hit(damage);
            return;
        }

        EnemyHealth enemy =
            hit.collider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(GetAttackDamage());
        }
    }

    private int GetResourceDamage(ResourceNode resource)
    {
        ItemData equippedItem =
            equipmentController != null
                ? equipmentController.CurrentItem
                : null;

        if (equippedItem == null)
        {
            return Mathf.Max(1, gatherDamage);
        }

        switch (resource.nodeType)
        {
            case ResourceNodeType.Tree:
                return Mathf.Max(
                    1,
                    equippedItem.treeDamage
                );

            case ResourceNodeType.Rock:
                return Mathf.Max(
                    1,
                    equippedItem.rockDamage
                );

            case ResourceNodeType.Ore:
                return Mathf.Max(
                    1,
                    equippedItem.oreDamage
                );

            case ResourceNodeType.Food:
                return Mathf.Max(1, gatherDamage);

            default:
                return Mathf.Max(1, gatherDamage);
        }
    }

    private int GetAttackDamage()
    {
        ItemData equippedItem =
            equipmentController != null
                ? equipmentController.CurrentItem
                : null;

        if (equippedItem == null)
        {
            return Mathf.Max(1, attackDamage);
        }

        return Mathf.Max(
            1,
            equippedItem.attackDamage
        );
    }
}