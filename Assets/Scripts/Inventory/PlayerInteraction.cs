using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private EquipmentController equipmentController;

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private Animator animator;

    [Header("Interaction")]
    [SerializeField]
    private float range = 3f;

    [SerializeField]
    private KeyCode interactKey = KeyCode.E;

    [SerializeField]
    private GameObject interactionPrompt;

    [Header("Combat")]
    [SerializeField]
    private float hitCooldown = 0.35f;

    [SerializeField]
    private int gatherDamage = 1;

    [SerializeField]
    private int attackDamage = 1;

    private float nextHitTime;

    private IInteractable currentInteractable;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera =
                GetComponentInChildren<Camera>();
        }

        if (equipmentController == null)
        {
            equipmentController =
                GetComponent<EquipmentController>();
        }

        if (playerStats == null)
        {
            playerStats =
                GetComponent<PlayerStats>();
        }

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
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

        if (playerStats == null)
        {
            Debug.LogError(
                "PlayerInteraction: PlayerStats was not found."
            );
        }

        if (animator == null)
        {
            Debug.LogWarning(
                "PlayerInteraction: Animator was not found."
            );
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Δεν εκτελούμε interactions
        // όταν κάποιο UI είναι ανοικτό.
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            HideInteractionPrompt();
            return;
        }

        UpdateInteractionPrompt();

        if (Input.GetMouseButtonDown(0))
        {
            TryUseItem();
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void UpdateInteractionPrompt()
    {
        if (playerCamera == null ||
            interactionPrompt == null)
        {
            return;
        }

        Ray ray =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        bool foundTarget =
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                range,
                ~0,
                QueryTriggerInteraction.Ignore
            );

        if (!foundTarget)
        {
            HideInteractionPrompt();
            return;
        }

        IInteractable interactable =
            hit.collider.GetComponentInParent<IInteractable>();

        if (interactable == null)
        {
            HideInteractionPrompt();
            return;
        }

        currentInteractable =
            interactable;

        ShowInteractionPrompt();
    }

    private void ShowInteractionPrompt()
    {
        if (interactionPrompt == null)
        {
            return;
        }

        if (!interactionPrompt.activeSelf)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void HideInteractionPrompt()
    {
        currentInteractable = null;

        if (interactionPrompt != null &&
            interactionPrompt.activeSelf)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            return;
        }

        Ray ray =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        bool foundTarget =
            Physics.Raycast(
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

    private void TryUseItem()
    {
        if (equipmentController == null)
        {
            return;
        }

        ItemData equippedItem =
            equipmentController.CurrentItem;

        if (equippedItem == null)
        {
            TryHit();
            return;
        }

        // -------------------------------------------------
        // FOOD
        // -------------------------------------------------

        if (equippedItem.itemType == ItemType.Food)
        {
            TryEat(equippedItem);
            return;
        }

        // -------------------------------------------------
        // PLACEABLE ITEMS
        // -------------------------------------------------

        if (equippedItem.isPlaceable)
        {
            return;
        }

        // -------------------------------------------------
        // TOOLS / WEAPONS
        // -------------------------------------------------

        TryHit();
    }

    private void TryEat(ItemData food)
    {
        if (food == null)
        {
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError(
                "PlayerInteraction: PlayerStats was not found."
            );

            return;
        }

        if (PlayerInventory.Instance == null)
        {
            Debug.LogError(
                "PlayerInteraction: PlayerInventory instance was not found."
            );

            return;
        }

        if (food.itemType != ItemType.Food)
        {
            return;
        }

        // Δεν τρώμε αν το hunger είναι ήδη γεμάτο.
        if (playerStats.hunger >= playerStats.maxHunger)
        {
            Debug.Log("Hunger is already full.");

            return;
        }

        // Αφαιρούμε πρώτα το Food από το inventory.
        bool removed =
            PlayerInventory.Instance.Remove(
                food,
                1
            );

        if (!removed)
        {
            Debug.LogWarning(
                $"Could not consume {food.displayName}."
            );

            return;
        }

        // Προσθέτουμε το hunger.
        playerStats.Eat(
            food.hungerRestore
        );

        Debug.Log(
            $"Ate {food.displayName}. " +
            $"Hunger restored: {food.hungerRestore}"
        );
    }

    private void TryHit()
    {
        if (playerCamera == null)
        {
            return;
        }

        ItemData equippedItem =
            equipmentController != null
                ? equipmentController.CurrentItem
                : null;

        // Όταν κρατάμε Workbench ή άλλο building,
        // το αριστερό κλικ χρησιμοποιείται μόνο για placement.
        if (equippedItem != null &&
            equippedItem.isPlaceable)
        {
            return;
        }

        if (Time.time < nextHitTime)
        {
            return;
        }

        nextHitTime =
            Time.time + hitCooldown;

        // -------------------------------------------------
        // ATTACK ANIMATION
        // -------------------------------------------------

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // -------------------------------------------------
        // HIT DETECTION
        // -------------------------------------------------

        Ray ray =
            new Ray(
                playerCamera.transform.position,
                playerCamera.transform.forward
            );

        bool foundTarget =
            Physics.Raycast(
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
            int damage =
                GetResourceDamage(resource);

            resource.Hit(damage);

            return;
        }

        EnemyHealth enemy =
            hit.collider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(
                GetAttackDamage()
            );
        }
    }

    private int GetResourceDamage(
        ResourceNode resource
    )
    {
        ItemData equippedItem =
            equipmentController != null
                ? equipmentController.CurrentItem
                : null;

        if (equippedItem == null)
        {
            return Mathf.Max(
                1,
                gatherDamage
            );
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

                return Mathf.Max(
                    1,
                    gatherDamage
                );

            default:

                return Mathf.Max(
                    1,
                    gatherDamage
                );
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
            return Mathf.Max(
                1,
                attackDamage
            );
        }

        return Mathf.Max(
            1,
            equippedItem.attackDamage
        );
    }
}