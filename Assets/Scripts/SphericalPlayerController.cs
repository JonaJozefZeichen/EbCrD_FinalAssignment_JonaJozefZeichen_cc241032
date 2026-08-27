using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SphericalPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 8.0f; // Sets base movement speed along spherical surface

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f; // Scales raw mouse delta so look controls feel smooth
    [SerializeField] private float verticalLookLimit = 85.0f; // Clamps camera pitch to prevent full upside-down flips
    [SerializeField] private Transform cameraTransform; // Points to camera to apply vertical pitch independently of body

    [Header("Planet Reference")]
    [SerializeField] private PlanetGravitySource planetGravity; // References planet center to pull and align player body

    private Rigidbody rb; // Handles physics velocity and collision resolution
    private InputSystem_Actions inputActions; // Holds reference to generated input asset
    private Vector2 moveInput; // Stores current WASD frame vector
    private Vector2 lookInput; // Stores current mouse delta frame vector
    private float cameraPitch = 0f; // Tracks vertical camera pitch in degrees

    private void Awake()
    {
        // Cache required rigidbody component
        rb = GetComponent<Rigidbody>();

        // Ensure gravity is disabled on rigidbody to prevent conflict with planet attractor
        rb.useGravity = false;

        // Instantiate input asset wrapper instance
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Enable player action map to process input events
        inputActions.Player.Enable();

        // Lock cursor to center of game window so mouse does not drift off screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        // Disable player action map to stop processing inputs while inactive
        inputActions.Player.Disable();

        // Release cursor lock so user can interact with editor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Read mouse and WASD input directly each frame for responsive controls
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        // Process vertical camera pitch and horizontal turning
        HandleLook();
    }

    private void FixedUpdate()
    {
        // Attract and align body to planet surface normal inside physics loop
        if (planetGravity != null)
        {
            planetGravity.Attract(rb);
        }

        // Apply tangential movement displacement along spherical surface
        HandleMovement();
    }

    private void HandleLook()
    {
        // Calculate horizontal yaw and vertical pitch deltas
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Apply horizontal yaw by rotating body around its current local Up axis
        transform.Rotate(0f, mouseX, 0f, Space.Self);

        // Clamp camera vertical pitch to prevent flipping
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);

        // Apply clamped pitch strictly to camera transform
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        // Clamp diagonal input magnitude to 1 so moving diagonally doesn't make the player run faster
        Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1.0f);

        // Calculate direction along player local surface tangents
        Vector3 moveDirection = (transform.forward * clampedInput.y + transform.right * clampedInput.x);

        // Displace position smoothly without overriding gravity fall velocity
        Vector3 displacement = moveDirection * walkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + displacement);
    }
}