using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private int maxHealth = 10;

    [Header("Loot")]
    [SerializeField]
    private List<LootEntry> lootTable =
        new List<LootEntry>();

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

        // Stop AI.
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        // Stop NavMeshAgent.
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        // Play Death animation.
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
        yield return new WaitForSeconds(
            deathAnimationDuration
        );

        DropLoot();

        Destroy(gameObject);
    }

    private void DropLoot()
    {
        ItemData selectedItem = SelectRandomLoot();

        if (selectedItem == null)
        {
            Debug.Log(
                $"{gameObject.name}: No loot was selected."
            );

            return;
        }

        if (selectedItem.worldPrefab == null)
        {
            Debug.LogError(
                $"Loot item '{selectedItem.displayName}' " +
                "does not have a World Prefab assigned."
            );

            return;
        }

        int minimum =
            Mathf.Max(1, minimumLootAmount);

        int maximum =
            Mathf.Max(minimum, maximumLootAmount);

        int amountToDrop =
            Random.Range(
                minimum,
                maximum + 1
            );

        Vector2 randomOffset =
            Random.insideUnitCircle *
            lootSpreadRadius;

        Vector3 spawnPosition =
            transform.position +
            new Vector3(
                randomOffset.x,
                lootHeightOffset,
                randomOffset.y
            );

        GameObject lootObject =
            Instantiate(
                selectedItem.worldPrefab,
                spawnPosition,
                Quaternion.identity
            );

        WorldItem worldItem =
            lootObject.GetComponent<WorldItem>();

        if (worldItem == null)
        {
            Debug.LogError(
                $"World prefab " +
                $"'{selectedItem.worldPrefab.name}' " +
                "does not contain a WorldItem component."
            );

            Destroy(lootObject);

            return;
        }

        worldItem.Initialize(
            selectedItem,
            amountToDrop
        );

        Debug.Log(
            $"Dropped {amountToDrop}x " +
            $"{selectedItem.displayName}."
        );
    }

    private ItemData SelectRandomLoot()
    {
        if (lootTable == null ||
            lootTable.Count == 0)
        {
            return null;
        }

        float totalChance = 0f;

        foreach (LootEntry entry in lootTable)
        {
            if (entry == null ||
                entry.item == null ||
                entry.chance <= 0f)
            {
                continue;
            }

            totalChance += entry.chance;
        }

        if (totalChance <= 0f)
        {
            return null;
        }

        float randomValue =
            Random.Range(0f, totalChance);

        float currentChance = 0f;

        foreach (LootEntry entry in lootTable)
        {
            if (entry == null ||
                entry.item == null ||
                entry.chance <= 0f)
            {
                continue;
            }

            currentChance += entry.chance;

            if (randomValue <= currentChance)
            {
                return entry.item;
            }
        }

        return null;
    }
}