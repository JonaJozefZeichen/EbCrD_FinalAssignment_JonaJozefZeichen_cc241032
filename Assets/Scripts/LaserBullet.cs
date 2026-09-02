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
    [SerializeField] private GameObject BulletParticels; // Spawns laserbullet particles trailing the projectile 

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        float stepDistance = speed * Time.deltaTime;
        Vector3 forward = transform.forward;
        Vector3 currentPosition = transform.position;

        // At 50 units/sec a plain position update can step clean through thin colliders between frames
        if (Physics.SphereCast(currentPosition, hitRadius, forward, out RaycastHit hitInfo, stepDistance, hitLayers))
        {
            transform.position = hitInfo.point;
            OnHit(hitInfo);
        }
        else
        {
            transform.position += forward * stepDistance;
        }
    }

    private void OnHit(RaycastHit hitInfo)
    {
        if (hitInfo.collider.TryGetComponent(out DestructibleObject destructible))
        {
            destructible.DestroyTarget(DestructibleObject.DestructionCause.LaserHit);
        }
        else
        {
            Destroy(hitInfo.collider.gameObject);
        }

        Destroy(gameObject);
    }
}