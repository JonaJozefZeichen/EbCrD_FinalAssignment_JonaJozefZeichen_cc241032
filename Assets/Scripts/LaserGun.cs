using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGun : MonoBehaviour
{
    [Header("Spawning References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform aimCamera;

    [Header("Weapon Cooldown")]
    [SerializeField] private float fireRate = 0.3f;

    [Header("Input Action")]
    [SerializeField] private InputActionReference shootAction;

    private float nextFireTime = 0f;

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

        Transform cameraSource = aimCamera != null ? aimCamera : transform;
        Vector3 targetPoint;

        if (Physics.Raycast(cameraSource.position, cameraSource.forward, out RaycastHit hitInfo, 200f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            targetPoint = cameraSource.position + (cameraSource.forward * 200f);
        }

        Vector3 aimDirection = (targetPoint - firePoint.position).normalized;
        Quaternion bulletRotation = Quaternion.LookRotation(aimDirection);

        Instantiate(bulletPrefab, firePoint.position, bulletRotation);
    }
}
