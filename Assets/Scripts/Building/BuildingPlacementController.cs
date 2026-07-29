using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private EquipmentController equipmentController;
    [SerializeField] private PlayerInventory inventory;

    [Header("Preview Materials")]
    [SerializeField] private Material validPreviewMaterial;
    [SerializeField] private Material invalidPreviewMaterial;

    [Header("Placement")]
    [SerializeField] private float placementDistance = 5f;
    [SerializeField] private float rotationStep = 15f;
    [SerializeField] private float maxSlopeAngle = 25f;

    [SerializeField] private LayerMask placementSurfaceMask = ~0;
    [SerializeField] private LayerMask obstacleMask = ~0;

    [Header("Input")]
    [SerializeField] private KeyCode rotateKey = KeyCode.R;

    private GameObject previewObject;
    private ItemData previewItem;

    private Renderer[] previewRenderers;
    private Collider[] previewColliders;
    private BoxCollider previewBoxCollider;

    private Vector3 currentPlacementPosition;
    private Quaternion currentPlacementRotation =
        Quaternion.identity;

    private Collider currentSurfaceCollider;

    private bool canPlace;
    private bool hasMaterialState;
    private bool lastMaterialState;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (inventory == null)
        {
            inventory = PlayerInventory.Instance;
        }
    }

    private void Update()
    {
        // Δεν επιτρέπεται placement όσο είναι ανοικτό κάποιο UI.
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            HidePreview();
            return;
        }

        ItemData selectedItem = GetSelectedItem();

        if (!IsPlaceableItem(selectedItem))
        {
            ClearPreview();
            return;
        }

        EnsurePreviewExists(selectedItem);

        if (Input.GetKeyDown(rotateKey))
        {
            RotatePreview();
        }

        UpdatePreviewPosition();

        if (canPlace && Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }
    }

    private ItemData GetSelectedItem()
    {
        if (equipmentController == null)
        {
            return null;
        }

        return equipmentController.CurrentItem;
    }

    private bool IsPlaceableItem(ItemData item)
    {
        return item != null &&
               item.isPlaceable &&
               item.placeablePrefab != null;
    }

    private void EnsurePreviewExists(ItemData item)
    {
        if (previewObject != null && previewItem == item)
        {
            return;
        }

        ClearPreview();

        previewItem = item;

        previewObject = Instantiate(
            previewItem.placeablePrefab
        );

        previewObject.name =
            $"{previewItem.displayName}_PlacementPreview";

        previewBoxCollider =
            previewObject.GetComponentInChildren<BoxCollider>(
                true
            );

        previewColliders =
            previewObject.GetComponentsInChildren<Collider>(
                true
            );

        previewRenderers =
            previewObject.GetComponentsInChildren<Renderer>(
                true
            );

        DisablePreviewColliders();
        DisablePreviewScripts();

        float cameraYaw = playerCamera != null
            ? playerCamera.transform.eulerAngles.y
            : transform.eulerAngles.y;

        float snappedYaw = Mathf.Round(
            cameraYaw / rotationStep
        ) * rotationStep;

        currentPlacementRotation =
            Quaternion.Euler(0f, snappedYaw, 0f);

        hasMaterialState = false;
    }

    private void DisablePreviewColliders()
    {
        if (previewColliders == null)
        {
            return;
        }

        foreach (Collider previewCollider in previewColliders)
        {
            if (previewCollider != null)
            {
                previewCollider.enabled = false;
            }
        }
    }

    private void DisablePreviewScripts()
    {
        if (previewObject == null)
        {
            return;
        }

        MonoBehaviour[] previewScripts =
            previewObject.GetComponentsInChildren<MonoBehaviour>(
                true
            );

        foreach (MonoBehaviour previewScript in previewScripts)
        {
            if (previewScript != null)
            {
                previewScript.enabled = false;
            }
        }
    }

    private void RotatePreview()
    {
        currentPlacementRotation *=
            Quaternion.Euler(0f, rotationStep, 0f);
    }

    private void UpdatePreviewPosition()
    {
        if (previewObject == null || playerCamera == null)
        {
            canPlace = false;
            return;
        }

        Ray placementRay =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        bool foundSurface = Physics.Raycast(
            placementRay,
            out RaycastHit hit,
            placementDistance,
            placementSurfaceMask,
            QueryTriggerInteraction.Ignore
        );

        if (!foundSurface)
        {
            canPlace = false;
            previewObject.SetActive(false);
            return;
        }

        previewObject.SetActive(true);

        currentPlacementPosition = hit.point;
        currentSurfaceCollider = hit.collider;

        previewObject.transform.SetPositionAndRotation(
            currentPlacementPosition,
            currentPlacementRotation
        );

        float surfaceAngle = Vector3.Angle(
            hit.normal,
            Vector3.up
        );

        bool validSlope =
            surfaceAngle <= maxSlopeAngle;

        bool areaIsClear =
            CheckPlacementAreaIsClear();

        canPlace = validSlope && areaIsClear;

        ApplyPreviewMaterial(canPlace);
    }

    private bool CheckPlacementAreaIsClear()
    {
        if (previewObject == null ||
            previewBoxCollider == null)
        {
            return true;
        }

        Vector3 boxCenter =
            previewObject.transform.TransformPoint(
                previewBoxCollider.center
            );

        Vector3 scale =
            previewObject.transform.lossyScale;

        Vector3 boxHalfExtents = new Vector3(
            previewBoxCollider.size.x *
            0.5f *
            Mathf.Abs(scale.x),

            previewBoxCollider.size.y *
            0.5f *
            Mathf.Abs(scale.y),

            previewBoxCollider.size.z *
            0.5f *
            Mathf.Abs(scale.z)
        );

        // Μικρή μείωση για να μην ακουμπά ο έλεγχος
        // οριακά το έδαφος.
        boxHalfExtents *= 0.95f;

        Collider[] overlappingColliders =
            Physics.OverlapBox(
                boxCenter,
                boxHalfExtents,
                previewObject.transform.rotation,
                obstacleMask,
                QueryTriggerInteraction.Ignore
            );

        foreach (Collider otherCollider
                 in overlappingColliders)
        {
            if (otherCollider == null)
            {
                continue;
            }

            // Αγνοούμε την επιφάνεια του εδάφους.
            if (otherCollider == currentSurfaceCollider)
            {
                continue;
            }

            // Αγνοούμε αντικείμενα του ίδιου του preview.
            if (otherCollider.transform.IsChildOf(
                    previewObject.transform))
            {
                continue;
            }

            // Αγνοούμε τον Player.
            if (otherCollider.transform.root ==
                transform.root)
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private void ApplyPreviewMaterial(bool isValid)
    {
        if (previewRenderers == null)
        {
            return;
        }

        if (hasMaterialState &&
            lastMaterialState == isValid)
        {
            return;
        }

        Material selectedMaterial =
            isValid
                ? validPreviewMaterial
                : invalidPreviewMaterial;

        if (selectedMaterial == null)
        {
            return;
        }

        foreach (Renderer previewRenderer
                 in previewRenderers)
        {
            if (previewRenderer == null)
            {
                continue;
            }

            Material[] materials =
                previewRenderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = selectedMaterial;
            }

            previewRenderer.sharedMaterials = materials;
        }

        lastMaterialState = isValid;
        hasMaterialState = true;
    }

    private void PlaceBuilding()
    {
        if (!canPlace ||
            previewItem == null ||
            inventory == null)
        {
            return;
        }

        bool removedFromInventory =
            inventory.Remove(previewItem, 1);

        if (!removedFromInventory)
        {
            Debug.LogWarning(
                $"Could not remove 1x " +
                $"{previewItem.displayName} from inventory."
            );

            return;
        }

        Instantiate(
            previewItem.placeablePrefab,
            currentPlacementPosition,
            currentPlacementRotation
        );

        Debug.Log(
            $"Placed {previewItem.displayName}."
        );

        ClearPreview();
    }

    private void HidePreview()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }

        canPlace = false;
    }

    private void ClearPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = null;
        previewItem = null;
        previewRenderers = null;
        previewColliders = null;
        previewBoxCollider = null;
        currentSurfaceCollider = null;

        canPlace = false;
        hasMaterialState = false;
    }

    private void OnDisable()
    {
        ClearPreview();
    }

    private void OnDestroy()
    {
        ClearPreview();
    }
}