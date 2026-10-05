using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// First-person 3D player controller: move, look, jump, sprint, crouch.
/// Setup:
///  1. Create an EMPTY GameObject (pivot at the player's feet) and add this script. A CharacterController is added automatically.
///     For a visible body, add a Capsule as a child (local Y = 1) and delete the child's Capsule Collider.
///     Don't put a Rigidbody or any extra collider on the player.
///  2. Make your Main Camera a child of the player and assign it to "Camera Pivot".
///  3. Uses the legacy Input Manager (Edit > Project Settings > Player > Active Input Handling = Both or Input Manager (Old)).
/// Controls: WASD move, Mouse look, Space jump, Left Shift sprint, Left Ctrl / C crouch.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float acceleration = 12f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundStickForce = 5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private float standCameraHeight = 1.7f;
    [SerializeField] private float crouchCameraHeight = 0.9f;
    [SerializeField] private bool toggleCrouch = false;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 85f;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float pitch;
    private float lastGroundedTime = -10f;
    private float lastJumpPressedTime = -10f;
    private bool isCrouching;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // A CharacterController is its own collider. A leftover CapsuleCollider (added by default
        // on Unity's Capsule object) or a non-kinematic Rigidbody fights with it and causes bouncing.
        CapsuleCollider extraCollider = GetComponent<CapsuleCollider>();
        if (extraCollider != null) Destroy(extraCollider);

        Rigidbody extraBody = GetComponent<Rigidbody>();
        if (extraBody != null) extraBody.isKinematic = true;

        controller.stepOffset = Mathf.Min(controller.stepOffset, crouchHeight * 0.4f);
        controller.height = standHeight;
        controller.center = new Vector3(0f, standHeight / 2f, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

    }

    private void Update()
    {
        if (Keyboard.current.leftAltKey.isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HandleLook();
        }

        HandleCrouch();
        HandleMovement();
    }

    private void HandleLook()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        pitch = Mathf.Clamp(pitch - mouseY, -maxLookAngle, maxLookAngle);
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleCrouch()
    {
        bool crouchPressed = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C);
        bool crouchHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

        bool wantsCrouch = toggleCrouch
            ? (crouchPressed ? !isCrouching : isCrouching)
            : crouchHeld;

        // Only stand up if there is room above the player.
        if (!wantsCrouch && isCrouching && !CanStandUp())
            wantsCrouch = true;

        isCrouching = wantsCrouch;

        // Smoothly change collider and camera height.
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        if (!Mathf.Approximately(controller.height, targetHeight))
        {
            float newHeight = Mathf.MoveTowards(
                controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
            controller.height = newHeight;
            controller.center = new Vector3(0f, newHeight / 2f, 0f);
        }

        if (cameraPivot != null)
        {
            float targetCamY = isCrouching ? crouchCameraHeight : standCameraHeight;
            Vector3 camPos = cameraPivot.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCamY, crouchTransitionSpeed * Time.deltaTime);
            cameraPivot.localPosition = camPos;
        }
    }

    private bool CanStandUp()
    {
        float radius = controller.radius * 0.95f;
        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 top = transform.position + Vector3.up * (standHeight - radius);
        // Ignore the player's own collider by checking against everything except it.
        Collider[] hits = Physics.OverlapCapsule(bottom, top, radius, ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            if (hit != controller) return false;
        }
        return true;
    }

    private void HandleMovement()
    {
        // Horizontal movement
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1f);

        bool wantsSprint = Input.GetKey(KeyCode.LeftShift) && v > 0f && !isCrouching;
        float targetSpeed = isCrouching ? crouchSpeed : (wantsSprint ? sprintSpeed : walkSpeed);

        Vector3 targetVelocity = transform.TransformDirection(input) * targetSpeed;
        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity, targetVelocity, acceleration * targetSpeed * Time.deltaTime);

        // Grounded / coyote time
        if (controller.isGrounded)
        {
            lastGroundedTime = Time.time;
            if (verticalVelocity < 0f) verticalVelocity = -groundStickForce; // keeps the controller stuck to the ground
        }

        // Jump (with buffer + coyote time)
        if (Input.GetButtonDown("Jump"))
            lastJumpPressedTime = Time.time;

        bool canCoyoteJump = Time.time - lastGroundedTime <= coyoteTime;
        bool jumpBuffered = Time.time - lastJumpPressedTime <= jumpBufferTime;

        if (jumpBuffered && canCoyoteJump && !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            lastJumpPressedTime = -10f;
            lastGroundedTime = -10f;
        }

        // Gravity
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}