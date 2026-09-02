using UnityEngine;

public class MeteoriteSpawner : MonoBehaviour
{
    [Header("Spawn Transforms")]
    [SerializeField] private Transform planetCenter; // Serves as the origin anchor and gravitational target for spawned objects
    [SerializeField] private GameObject meteoritePrefab; // References meteorite prefab to instantiate into orbit

    [Header("Spherical Spawn Settings")]
    [SerializeField] private float spawnRadius = 35.0f; // Sets spherical distance from planet center where meteorites appear
    [SerializeField] private float minSpawnInterval = 1.5f; // Sets shortest possible delay between spawns
    [SerializeField] private float maxSpawnInterval = 4.0f; // Sets longest possible delay between spawns

    [Header("Gizmo Visualizer")]
    [SerializeField] private bool showGizmos = true; // Toggles editor wireframe sphere for visual debugging
    [SerializeField] private Color gizmoColor = new Color(1f, 0.5f, 0f, 0.3f); // Defines editor display color for spawn boundary

    private float nextSpawnTime = 0f; // Tracks timestamp when next meteorite should instantiate

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
        // Randomized so spawns don't fall into an obvious, learnable rhythm
        float randomDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomDelay;
    }

    private void SpawnMeteorite()
    {
        if (meteoritePrefab == null) return;

        // Random.onUnitSphere already gives a uniform direction, just scale it out to the shell radius
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 spawnPosition = planetCenter.position + (randomDirection * spawnRadius);

        // Face inward on spawn so it doesn't visibly snap-rotate on the first movement tick
        Vector3 inwardDirection = (planetCenter.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(inwardDirection);

        GameObject spawnedObject = Instantiate(meteoritePrefab, spawnPosition, spawnRotation);

        if (spawnedObject.TryGetComponent(out MeteoriteMovement movementComponent))
        {
            movementComponent.SetGravityTarget(planetCenter);
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