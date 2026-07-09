using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float attackRange = 2f;
    public float attackCooldown = 1.2f;
    public float damage = 10f;

    private NavMeshAgent agent;
    private PlayerStats playerStats;

    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
                playerStats = playerObject.GetComponent<PlayerStats>();
            }
        }
        else
        {
            playerStats = target.GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        if (target == null)
            return;

        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            if (playerStats != null)
            {
                playerStats.Damage(damage);
                Debug.Log("Enemy attacked player!");
            }
        }
    }
}