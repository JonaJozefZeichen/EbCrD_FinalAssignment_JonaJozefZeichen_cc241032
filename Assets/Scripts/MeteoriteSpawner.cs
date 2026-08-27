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
        // Default to self transform if planet center is not explicitly assigned
        if (planetCenter == null)
        {
            planetCenter = transform;
        }

        // Schedule first spawn timestamp
        ScheduleNextSpawn();
    }

    private void Update()
    {
        // Check if spawn timer elapsed
        if (Time.time >= nextSpawnTime)
        {
            // Spawn meteorite object into scene
            SpawnMeteorite();

            // Calculate new randomized interval for upcoming spawn
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        // Randomize delay to prevent predictable spawn patterns
        float randomDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomDelay;
    }

    private void SpawnMeteorite()
    {
        // Abort if prefab reference is missing to prevent null reference errors
        if (meteoritePrefab == null) return;

        // Generate normalized random 3D direction on outer surface of unit sphere
        Vector3 randomDirection = Random.onUnitSphere;

        // Calculate spawn coordinates at exact shell radius from planet center
        Vector3 spawnPosition = planetCenter.position + (randomDirection * spawnRadius);

        // Orient meteorite to face directly toward planet center upon entry
        Vector3 inwardDirection = (planetCenter.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(inwardDirection);

        // Instantiate meteorite at outer boundary
        GameObject spawnedObject = Instantiate(meteoritePrefab, spawnPosition, spawnRotation);

        // Pass planet center reference to meteorite logic if attraction component is present
        MeteoriteMovement movementComponent = spawnedObject.GetComponent<MeteoriteMovement>();
        if (movementComponent != null)
        {
            movementComponent.SetGravityTarget(planetCenter);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw wireframe sphere in editor to visualize spawn boundary
        if (!showGizmos) return;

        Transform centerTransform = planetCenter != null ? planetCenter : transform;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(centerTransform.position, spawnRadius);
    }
}