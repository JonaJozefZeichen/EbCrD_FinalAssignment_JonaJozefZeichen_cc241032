using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    [Header("Flight Properties")]
    [SerializeField] private float speed = 50.0f;
    [SerializeField] private float lifetime = 3.0f;
    [SerializeField] private float hitRadius = 0.1f;

    [Header("Hit Filters")]
    [SerializeField] private LayerMask hitLayers = ~0;

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

        Destroy(gameObject);
    }
}
