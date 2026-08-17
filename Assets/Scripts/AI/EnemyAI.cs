using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public Transform target;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.2f;
    public float damage = 10f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    // Το συγκεκριμένο Goblin model έχει το forward του
    // στραμμένο προς την αντίθετη κατεύθυνση.
    private const float ModelRotationOffset = 180f;

    private NavMeshAgent agent;
    private PlayerStats playerStats;
    private Animator animator;

    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Το EnemyAI ελέγχει το rotation.
        agent.updateRotation = false;
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;

                playerStats =
                    playerObject.GetComponent<PlayerStats>();
            }
        }
        else
        {
            playerStats =
                target.GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        if (target == null)
        {
            SetMovingAnimation(false);
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                target.position
            );

        // -------------------------
        // Movement
        // -------------------------

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);

            SetMovingAnimation(
                agent.velocity.sqrMagnitude > 0.01f
            );
        }
        else
        {
            // Σταματάμε όταν φτάσουμε αρκετά κοντά.
            agent.isStopped = true;

            SetMovingAnimation(false);
        }

        // -------------------------
        // Rotation
        // -------------------------

        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            // Το Goblin model είναι γυρισμένο 180°.
            targetRotation *=
                Quaternion.Euler(
                    0f,
                    ModelRotationOffset,
                    0f
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }

        // -------------------------
        // Attack
        // -------------------------

        if (distance <= attackRange &&
            Time.time >= nextAttackTime)
        {
            nextAttackTime =
                Time.time + attackCooldown;

            // Παίζει το Attack animation.
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // Κάνει damage στον Player.
            if (playerStats != null)
            {
                playerStats.Damage(damage);

                Debug.Log("Enemy attacked player!");
            }
        }
    }

    private void SetMovingAnimation(bool moving)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsMoving", moving);
    }
}