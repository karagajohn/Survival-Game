using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private int maxHealth = 10;

    [Header("Loot")]
    [SerializeField]
    private GameObject lootPrefab;

    [SerializeField]
    [Min(0)]
    private int minimumLootAmount = 1;

    [SerializeField]
    [Min(0)]
    private int maximumLootAmount = 2;

    [SerializeField]
    [Min(0f)]
    private float lootSpreadRadius = 0.5f;

    [SerializeField]
    private float lootHeightOffset = 0.25f;

    [Header("Death")]
    [SerializeField]
    private float deathAnimationDuration = 1.5f;

    private int currentHealth;
    private bool isDead;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private EnemyAI enemyAI;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(
            $"{gameObject.name} took {amount} damage. " +
            $"HP: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        // Σταματάμε το AI.
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        // Σταματάμε το NavMeshAgent.
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        // Παίζουμε το Death animation.
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Death");
        }

        Debug.Log($"{gameObject.name} died.");

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Περιμένουμε να ολοκληρωθεί το Death animation.
        yield return new WaitForSeconds(deathAnimationDuration);

        DropLoot();

        Destroy(gameObject);
    }

    private void DropLoot()
    {
        if (lootPrefab == null)
        {
            return;
        }

        int minimum = Mathf.Max(0, minimumLootAmount);
        int maximum = Mathf.Max(minimum, maximumLootAmount);

        int amountToDrop = Random.Range(
            minimum,
            maximum + 1
        );

        for (int i = 0; i < amountToDrop; i++)
        {
            Vector2 randomOffset =
                Random.insideUnitCircle * lootSpreadRadius;

            Vector3 spawnPosition =
                transform.position +
                new Vector3(
                    randomOffset.x,
                    lootHeightOffset,
                    randomOffset.y
                );

            Instantiate(
                lootPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }
}