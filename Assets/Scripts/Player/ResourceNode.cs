using UnityEngine;

public enum ResourceNodeType
{
    Tree,
    Rock,
    Ore,
    Food
}

public class ResourceNode : MonoBehaviour
{
    public ResourceNodeType nodeType =
        ResourceNodeType.Tree;

    public ResourceKind resourceKind =
        ResourceKind.Wood;

    public int maxHealth = 3;
    public int dropAmount = 3;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(
        ResourceKind kind,
        ResourceNodeType type,
        int health,
        int amount
    )
    {
        resourceKind = kind;
        nodeType = type;
        maxHealth = health;
        dropAmount = amount;
        currentHealth = maxHealth;
    }

    public void Hit(int damage)
    {
        currentHealth -= damage;

        Debug.Log(
            $"{gameObject.name} hit. HP: {currentHealth}"
        );

        PlayHitSound();

        if (currentHealth <= 0)
        {
            Harvest();
        }
    }

    private void PlayHitSound()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        switch (nodeType)
        {
            case ResourceNodeType.Tree:

                AudioManager.Instance.PlayWoodHit();

                break;

            case ResourceNodeType.Rock:
            case ResourceNodeType.Ore:

                AudioManager.Instance.PlayStoneHit();

                break;
        }
    }

    private void Harvest()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.Add(
                resourceKind,
                dropAmount
            );
        }

        Destroy(gameObject);
    }
}