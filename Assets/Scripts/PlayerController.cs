using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f; // Sets base walking speed in units per second
    [SerializeField] private float gravity = -9.81f; // Pulls player down toward the ground
    [SerializeField] private float groundedGravity = -2.0f; // Snaps player to ground so isGrounded stays reliable on slopes

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 0.1f; // Scales raw mouse delta so camera rotation feels natural
    [SerializeField] private float verticalLookLimit = 85.0f; // Clamps camera pitch to prevent upside-down camera flips
    [SerializeField] private Transform cameraTransform; // Points to camera to tilt up and down independently of body

    [Header("Raycast Settings")]
    [SerializeField] private float interactionDistance = 5.0f; // Limits how far player can reach objects
    [SerializeField] private LayerMask interactableLayer = ~0; // Filters which physics layers raycast hits to avoid checking unwanted colliders

    private CharacterController characterController; // Handles movement collisions and slope stepping
    private InputSystem_Actions inputActions; // Holds reference to generated input asset
    private float verticalRotation = 0f; // Tracks current camera pitch in degrees
    private float verticalVelocity = 0f; // Stores accumulated downward speed from gravity

    private void Awake()
    {
        // Cache required character controller component
        characterController = GetComponent<CharacterController>();

        // Instantiate input asset to read hardware events
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Enable input asset map to start processing events
        inputActions.Player.Enable();

        // Lock cursor to center so mouse does not leave game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        // Disable input asset map to prevent processing inputs while inactive
        inputActions.Player.Disable();

        // Release cursor lock so user can interact with editor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Process rotation before translation so forward vector is up to date
        HandleLook();

        // Process ground movement and downward gravity displacement
        HandleMovement();

        // Check for interactable objects in player crosshair
        HandleRaycast();
    }

    private void HandleLook()
    {
        // Read mouse delta directly each frame for responsive rotation
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        // Scale mouse input delta by sensitivity
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        // Rotate entire player body horizontally around Y axis so forward facing updates
        transform.Rotate(Vector3.up * mouseX);

        // Clamp camera vertical pitch to prevent flipping
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        // Apply clamped pitch rotation to camera
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        // Read continuous WASD input directly each frame to avoid callback drops
        Vector2 rawInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Clamp diagonal input magnitude to 1 so moving diagonally doesn't make the player run faster
        Vector2 moveInput = Vector2.ClampMagnitude(rawInput, 1.0f);

        // Convert 2D input coordinates into 3D direction relative to player body orientation
        Vector3 moveDirection = (transform.right * moveInput.x) + (transform.forward * moveInput.y);

        // Calculate horizontal velocity vector
        Vector3 finalVelocity = moveDirection * moveSpeed;

        // Keep slight downward force while grounded to maintain ground contact on slopes
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedGravity;
        }
        else
        {
            // Accumulate downward acceleration over time while airborne
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Apply vertical speed into velocity vector
        finalVelocity.y = verticalVelocity;

        // Move character controller through physics simulation
        characterController.Move(finalVelocity * Time.deltaTime);
    }

    private void HandleRaycast()
    {
        // Skip raycasting if camera reference is missing to prevent null reference errors
        if (cameraTransform == null) return;

        // Define origin and direction straight out from camera lens
        Vector3 rayOrigin = cameraTransform.position;
        Vector3 rayDirection = cameraTransform.forward;

        // Cast ray into scene to detect hit surfaces
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hitInfo, interactionDistance, interactableLayer))
        {
            // Draw green line in scene view when pointing at valid target
            Debug.DrawRay(rayOrigin, rayDirection * hitInfo.distance, Color.green);
        }
        else
        {
            // Draw red line in scene view when pointing into empty air
            Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red);
        }
    }
}