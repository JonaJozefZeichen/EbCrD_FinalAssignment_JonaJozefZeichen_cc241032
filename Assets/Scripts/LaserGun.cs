using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGun : MonoBehaviour
{
    [Header("Spawning References")]
    [SerializeField] private GameObject bulletPrefab; // References laser projectile prefab to instantiate on fire
    [SerializeField] private Transform firePoint; // Defines muzzle exit point where bullet spawns in world space
    [SerializeField] private Transform aimCamera; // Directs bullet trajectory toward center of screen

    [Header("Weapon Cooldown")]
    [SerializeField] private float fireRate = 0.3f; // Sets minimum time delay between consecutive shots in seconds

    [Header("Input Action")]
    [SerializeField] private InputActionReference shootAction; // Reads trigger input to fire weapon

    private float nextFireTime = 0f; // Stores game timestamp when gun is allowed to shoot again

    private void OnEnable()
    {
        if (shootAction != null)
        {
            shootAction.action.performed += OnShootInput;
            shootAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // Unsubscribe or the delegate keeps a reference and leaks the gun
        if (shootAction != null)
        {
            shootAction.action.performed -= OnShootInput;
            shootAction.action.Disable();
        }
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Fall back to own transform if no aim camera was assigned in the inspector
        Transform cameraSource = aimCamera != null ? aimCamera : transform;
        Vector3 targetPoint;

        // Raycast from the camera, not the muzzle, so aim always matches what's on screen
        if (Physics.Raycast(cameraSource.position, cameraSource.forward, out RaycastHit hitInfo, 200f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            // Nothing hit within range, aim far out so the bullet still flies straight
            targetPoint = cameraSource.position + (cameraSource.forward * 200f);
        }

        // Aim from the muzzle toward that point, not straight ahead, so the bullet converges on the crosshair instead of drifting with the muzzle offset
        Vector3 aimDirection = (targetPoint - firePoint.position).normalized;
        Quaternion bulletRotation = Quaternion.LookRotation(aimDirection);

        Instantiate(bulletPrefab, firePoint.position, bulletRotation);
    }
}