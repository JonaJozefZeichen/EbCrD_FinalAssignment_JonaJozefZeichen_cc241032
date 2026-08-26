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
        // Bind input action callback to shoot trigger
        if (shootAction != null)
        {
            shootAction.action.performed += OnShootInput;
            shootAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // Unbind input action callback to prevent memory leaks
        if (shootAction != null)
        {
            shootAction.action.performed -= OnShootInput;
            shootAction.action.Disable();
        }
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        // Block shot attempt if weapon is still cooling down
        if (Time.time < nextFireTime) return;

        // Update cooldown timer before executing shot logic
        nextFireTime = Time.time + fireRate;

        // Spawn and orient laser bullet
        Shoot();
    }

    private void Shoot()
    {
        // Abort if bullet prefab or spawn point is unassigned to prevent null reference crashes
        if (bulletPrefab == null || firePoint == null) return;

        // Fall back to current transform if aim camera is unassigned
        Transform cameraSource = aimCamera != null ? aimCamera : transform;
        Vector3 targetPoint;

        // Raycast forward from camera center to find what the player is aiming at
        if (Physics.Raycast(cameraSource.position, cameraSource.forward, out RaycastHit hitInfo, 200f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            // Project point into far distance so bullets still travel forward when aiming into empty space
            targetPoint = cameraSource.position + (cameraSource.forward * 200f);
        }

        // Calculate direction vector from muzzle to target point so bullet converges on crosshair
        Vector3 aimDirection = (targetPoint - firePoint.position).normalized;
        Quaternion bulletRotation = Quaternion.LookRotation(aimDirection);

        // Instantiate bullet prefab at muzzle with aligned trajectory
        Instantiate(bulletPrefab, firePoint.position, bulletRotation);
    }
}