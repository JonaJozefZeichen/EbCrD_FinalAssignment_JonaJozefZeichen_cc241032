using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    [Header("Flight Properties")]
    [SerializeField] private float speed = 50.0f; // Controls projectile travel velocity in units per second
    [SerializeField] private float lifetime = 3.0f; // Destroys bullet after set time to prevent infinite memory leaks in empty space
    [SerializeField] private float hitRadius = 0.1f; // Defines thickness of collision sphere to detect close impacts

    [Header("Hit Filters")]
    [SerializeField] private LayerMask hitLayers = ~0; // Filters collision checks so projectile only triggers on target layers

    [Header("Visual Effects")]
    [SerializeField] private GameObject impactEffectPrefab; // Spawns hit spark or particle effect at point of impact

    private void Start()
    {
        // Schedule object destruction so missed shots do not clutter hierarchy
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move projectile forward in local space
        MoveBullet();
    }

    private void MoveBullet()
    {
        // Calculate frame travel displacement
        float stepDistance = speed * Time.deltaTime;
        Vector3 forward = transform.forward;
        Vector3 currentPosition = transform.position;

        // Perform spherecast along trajectory to prevent tunneling through thin geometry at high speeds
        if (Physics.SphereCast(currentPosition, hitRadius, forward, out RaycastHit hitInfo, stepDistance, hitLayers))
        {
            // Position bullet directly at impact surface
            transform.position = hitInfo.point;

            // Trigger impact resolution logic
            OnHit(hitInfo);
        }
        else
        {
            // Advance bullet position when no obstacle is hit
            transform.position += forward * stepDistance;
        }
    }

    private void OnHit(RaycastHit hitInfo)
    {
        // Spawn impact particle oriented along surface normal to make sparks bounce outward
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        }

        // Check for destructible component on hit object
        DestructibleObject destructible = hitInfo.collider.GetComponent<DestructibleObject>();

        // Destroy target directly if script exists, otherwise remove raw GameObject
        if (destructible != null)
        {
            destructible.DestroyTarget();
        }
        else
        {
            Destroy(hitInfo.collider.gameObject);
        }

        // Remove projectile from scene upon impact
        Destroy(gameObject);
    }
}