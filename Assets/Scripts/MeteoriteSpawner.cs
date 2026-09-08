using UnityEngine;

public class MeteoriteSpawner : MonoBehaviour
{
    [Header("Spawn Transforms")]
    [SerializeField] private Transform planetCenter;
    [SerializeField] private SphereCollider planetCollider;
    [SerializeField] private GameObject meteoritePrefab;

    [Header("Spherical Spawn Settings")]
    [SerializeField] private float spawnRadius = 35.0f;
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float maxSpawnInterval = 4.0f;

    [Header("Gizmo Visualizer")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.5f, 0f, 0.3f);

    private float nextSpawnTime = 0f;

    private void Start()
    {
        if (planetCenter == null)
        {
            planetCenter = transform;
        }

        ScheduleNextSpawn();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnMeteorite();
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        float randomDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomDelay;
    }

    private void SpawnMeteorite()
    {
        if (meteoritePrefab == null) return;

        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 spawnPosition = planetCenter.position + (randomDirection * spawnRadius);

        Vector3 inwardDirection = (planetCenter.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(inwardDirection);

        GameObject spawnedObject = Instantiate(meteoritePrefab, spawnPosition, spawnRotation);

        if (spawnedObject.TryGetComponent(out MeteoriteMovement movementComponent))
        {
            movementComponent.SetGravityTarget(planetCenter);
        }

        if (spawnedObject.TryGetComponent(out DestructibleObject destructibleComponent))
        {
            destructibleComponent.SetPlanetCollider(planetCollider);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Transform centerTransform = planetCenter != null ? planetCenter : transform;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(centerTransform.position, spawnRadius);
    }
}
