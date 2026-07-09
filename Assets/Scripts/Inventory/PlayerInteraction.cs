using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextHitTime)
        {
            nextHitTime = Time.time + hitCooldown;
            TryHit();
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
                resource.Hit(gatherDamage);
                return;
            }

            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                return;
            }
        }
    }

    public void UpgradeToStoneAxe()
    {
        gatherDamage = Mathf.Max(gatherDamage, 4);
        attackDamage = Mathf.Max(attackDamage, 2);
        Debug.Log("Equipped Stone Axe!");
    }

    public void UpgradeToClub()
    {
        attackDamage = Mathf.Max(attackDamage, 5);
        Debug.Log("Equipped Club!");
    }
}