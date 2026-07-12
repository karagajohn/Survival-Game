using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public EquipmentController equipmentController;
    public Camera playerCamera;

    public float range = 3f;
    public float hitCooldown = 0.35f;

    public int gatherDamage = 1;
    public int attackDamage = 1;

    private float nextHitTime;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
            if (equipmentController == null)
            {
                equipmentController =
                    GetComponent<EquipmentController>();
            }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryHit();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f,0.5f)
        );

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            range))
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
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            ResourceNode resource = hit.collider.GetComponentInParent<ResourceNode>();

            if (resource != null)
            {
                int damage = GetResourceDamage(resource);
                resource.Hit(damage);
                return;
            }

            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(GetAttackDamage());
                return;
            }
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
            return 1;
        }

        switch (resource.nodeType)
        {
            case ResourceNodeType.Tree:
                return Mathf.Max(1, equippedItem.treeDamage);

            case ResourceNodeType.Rock:
                return Mathf.Max(1, equippedItem.rockDamage);

            case ResourceNodeType.Ore:
                return Mathf.Max(1, equippedItem.oreDamage);

            case ResourceNodeType.Food:
                return 1;

            default:
                return 1;
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
            return 1;
        }

        return Mathf.Max(1, equippedItem.attackDamage);
    }
}