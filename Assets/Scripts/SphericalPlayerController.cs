using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SphericalPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 8.0f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float verticalLookLimit = 85.0f;
    [SerializeField] private Transform cameraTransform;

    [Header("Planet Reference")]
    [SerializeField] private PlanetGravitySource planetGravity;

    private Rigidbody rb;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

        transform.Rotate(0f, mouseX, 0f, Space.Self);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1.0f);

        Vector3 moveDirection = (transform.forward * clampedInput.y + transform.right * clampedInput.x);

        Vector3 displacement = moveDirection * walkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + displacement);
    }
}
