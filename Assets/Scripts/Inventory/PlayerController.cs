using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private bool inputEnabled = true;

    public bool InputEnabled => inputEnabled;

    [Header("References")]
    public Transform cameraTransform;

    [SerializeField]
    private Animator animator;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float sprintStaminaPerSecond = 25f;

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;

    [Range(0f, 0.2f)]
    public float mouseSmoothTime = 0.04f;

    public float minimumLookAngle = -80f;
    public float maximumLookAngle = 80f;

    private CharacterController controller;
    private PlayerStats stats;

    private Vector3 verticalVelocity;

    private Vector2 currentMouseDelta;
    private Vector2 mouseDeltaVelocity;

    private float cameraPitch;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();

        FindCameraIfMissing();
        FindAnimatorIfMissing();

        LockCursor();
    }

    private void Update()
    {
        if (!inputEnabled)
        {
            ResetAnimator();
            return;
        }

        HandleCursor();

        // =====================================================
        // DEATH
        // =====================================================

        if (stats != null && stats.IsDead)
        {
            UpdateDeathAnimation();
            return;
        }

        // =====================================================
        // NORMAL GAMEPLAY
        // =====================================================

        Look();
        Move();
    }

    // =========================================================
    // REFERENCES
    // =========================================================

    private void FindCameraIfMissing()
    {
        if (cameraTransform != null)
        {
            return;
        }

        Camera playerCamera =
            GetComponentInChildren<Camera>();

        if (playerCamera != null)
        {
            cameraTransform =
                playerCamera.transform;
        }
        else
        {
            Debug.LogError(
                "PlayerController: No player camera found."
            );
        }
    }

    private void FindAnimatorIfMissing()
    {
        if (animator != null)
        {
            return;
        }

        animator =
            GetComponentInChildren<Animator>();

        if (animator != null)
        {
            Debug.Log(
                "PlayerController: Animator found automatically."
            );
        }
        else
        {
            Debug.LogError(
                "PlayerController: No Animator found in Player children."
            );
        }
    }

    // =========================================================
    // CAMERA LOOK
    // =========================================================

    private void Look()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector2 targetMouseDelta =
            new Vector2(
                Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y")
            ) * mouseSensitivity;

        currentMouseDelta =
            Vector2.SmoothDamp(
                currentMouseDelta,
                targetMouseDelta,
                ref mouseDeltaVelocity,
                mouseSmoothTime
            );

        cameraPitch -= currentMouseDelta.y;

        cameraPitch =
            Mathf.Clamp(
                cameraPitch,
                minimumLookAngle,
                maximumLookAngle
            );

        cameraTransform.localRotation =
            Quaternion.Euler(
                cameraPitch,
                0f,
                0f
            );

        transform.Rotate(
            Vector3.up *
            currentMouseDelta.x
        );
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void Move()
    {
        bool isGrounded =
            controller.isGrounded;

        if (animator != null)
        {
            animator.SetBool(
                "IsGrounded",
                isGrounded
            );
        }

        if (isGrounded &&
            verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        Vector2 movementInput =
            new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

        movementInput =
            Vector2.ClampMagnitude(
                movementInput,
                1f
            );

        Vector3 movementDirection =
            transform.right *
            movementInput.x +
            transform.forward *
            movementInput.y;

        float movementSpeed =
            GetMovementSpeed(
                movementDirection
            );

        controller.Move(
            movementDirection *
            movementSpeed *
            Time.deltaTime
        );

        UpdateMovementAnimation(
            movementInput,
            movementSpeed
        );

        HandleJump(isGrounded);

        verticalVelocity.y +=
            gravity *
            Time.deltaTime;

        controller.Move(
            verticalVelocity *
            Time.deltaTime
        );
    }

    private float GetMovementSpeed(
        Vector3 movementDirection
    )
    {
        bool isMoving =
            movementDirection.sqrMagnitude >
            0.01f;

        bool wantsToSprint =
            Input.GetKey(KeyCode.LeftShift) &&
            isMoving;

        if (!wantsToSprint)
        {
            return walkSpeed;
        }

        if (stats == null)
        {
            return sprintSpeed;
        }

        bool hasStamina =
            stats.UseStamina(
                sprintStaminaPerSecond *
                Time.deltaTime
            );

        return hasStamina
            ? sprintSpeed
            : walkSpeed;
    }

    // =========================================================
    // ANIMATION - NORMAL MOVEMENT
    // =========================================================

    private void UpdateMovementAnimation(
        Vector2 movementInput,
        float movementSpeed
    )
    {
        if (animator == null)
        {
            return;
        }

        // Player is alive again.
        animator.SetBool(
            "IsDead",
            false
        );

        float speed =
            movementInput.magnitude;

        bool isMoving =
            speed > 0.1f;

        bool isSprinting =
            isMoving &&
            movementSpeed >=
            sprintSpeed - 0.01f;

        animator.SetFloat(
            "Speed",
            speed
        );

        animator.SetBool(
            "Sprint",
            isSprinting
        );
    }

    // =========================================================
    // ANIMATION - DEATH
    // =========================================================

    private void UpdateDeathAnimation()
    {
        if (animator == null)
        {
            Debug.LogError(
                "PlayerController: Animator is NULL during death!"
            );

            return;
        }

        // Stop movement animation values.
        animator.SetFloat(
            "Speed",
            0f
        );

        animator.SetBool(
            "Sprint",
            false
        );

        animator.SetBool(
            "IsGrounded",
            true
        );

        // This is the ONLY thing that tells
        // the Animator Controller to enter Death.
        animator.SetBool(
            "IsDead",
            true
        );

        Debug.Log(
            "Death animation requested. Animator IsDead = " +
            animator.GetBool("IsDead")
        );
    }

    private void ResetAnimator()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(
            "Speed",
            0f
        );

        animator.SetBool(
            "Sprint",
            false
        );

        animator.SetBool(
            "IsGrounded",
            true
        );
    }

    // =========================================================
    // JUMP
    // =========================================================

    private void HandleJump(
        bool isGrounded
    )
    {
        if (!isGrounded)
        {
            return;
        }

        if (!Input.GetButtonDown("Jump"))
        {
            return;
        }

        verticalVelocity.y =
            Mathf.Sqrt(
                jumpHeight *
                -2f *
                gravity
            );
    }

    // =========================================================
    // CURSOR
    // =========================================================

    private void HandleCursor()
    {
        if (Input.GetMouseButtonDown(0) &&
            Cursor.lockState !=
            CursorLockMode.Locked)
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    // =========================================================
    // PUBLIC CONTROL
    // =========================================================

    public void ResetMotion()
    {
        verticalVelocity =
            Vector3.zero;

        currentMouseDelta =
            Vector2.zero;

        mouseDeltaVelocity =
            Vector2.zero;

        ResetAnimator();
    }

    public void SetInputEnabled(
        bool enabled
    )
    {
        inputEnabled = enabled;

        currentMouseDelta =
            Vector2.zero;

        mouseDeltaVelocity =
            Vector2.zero;

        if (!enabled)
        {
            ResetAnimator();
        }

        if (enabled)
        {
            LockCursor();
        }
    }
}