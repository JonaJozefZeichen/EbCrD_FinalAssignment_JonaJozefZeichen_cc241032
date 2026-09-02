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
        rb = GetComponent<Rigidbody>();

        // Built-in gravity would fight PlanetGravitySource's own pull direction
        rb.useGravity = false;

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Polled here instead of via callback so held keys don't drop a frame between input events
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        HandleLook();
    }

    private void FixedUpdate()
    {
        if (planetGravity != null)
        {
            planetGravity.Attract(rb);
        }

        HandleMovement();
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Yaw turns the whole body around its own local up, whatever that currently is on the sphere
        transform.Rotate(0f, mouseX, 0f, Space.Self);

        // Pitch only ever touches the camera, not the body, so looking up/down can't tilt the walk direction
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        // W+D unclamped would add up to sqrt(2) speed, clamp to 1 so diagonals aren't faster
        Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1.0f);

        Vector3 moveDirection = (transform.forward * clampedInput.y + transform.right * clampedInput.x);

        // MovePosition instead of adding velocity, so this can't stack with the gravity force each frame
        Vector3 displacement = moveDirection * walkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + displacement);
    }
}