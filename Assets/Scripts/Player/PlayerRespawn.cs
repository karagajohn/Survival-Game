using System.Collections;
using UnityEngine;

[RequireComponent(
    typeof(PlayerStats),
    typeof(CharacterController),
    typeof(PlayerController)
)]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;

    [Min(0f)]
    public float respawnDelay = 2f;

    [Range(0f, 1f)]
    public float respawnHungerPercentage = 0.7f;

    private PlayerStats stats;
    private PlayerController playerController;
    private CharacterController characterController;

    private Coroutine respawnCoroutine;

    private void Awake()
    {
        stats =
            GetComponent<PlayerStats>();

        playerController =
            GetComponent<PlayerController>();

        characterController =
            GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (stats != null)
        {
            stats.OnDied += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (stats != null)
        {
            stats.OnDied -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        if (respawnCoroutine != null)
        {
            return;
        }

        respawnCoroutine =
            StartCoroutine(
                RespawnRoutine()
            );
    }

    private IEnumerator RespawnRoutine()
    {
        // -----------------------------------------------------
        // PLAYER REMAINS DEAD
        // -----------------------------------------------------

        yield return new WaitForSeconds(
            respawnDelay
        );

        // -----------------------------------------------------
        // DISABLE CHARACTER CONTROLLER
        // -----------------------------------------------------

        characterController.enabled = false;

        // -----------------------------------------------------
        // MOVE TO RESPAWN POINT
        // -----------------------------------------------------

        if (respawnPoint != null)
        {
            transform.SetPositionAndRotation(
                respawnPoint.position,
                respawnPoint.rotation
            );
        }
        else
        {
            Debug.LogWarning(
                "PlayerRespawn: No respawn point assigned."
            );
        }

        // -----------------------------------------------------
        // RESET PLAYER MOTION
        // -----------------------------------------------------

        playerController.ResetMotion();

        // -----------------------------------------------------
        // ENABLE CHARACTER CONTROLLER
        // -----------------------------------------------------

        characterController.enabled = true;

        // -----------------------------------------------------
        // REVIVE PLAYER
        // -----------------------------------------------------

        stats.Respawn(
            respawnHungerPercentage
        );

        // -----------------------------------------------------
        // FINISHED
        // -----------------------------------------------------

        respawnCoroutine = null;
    }
}