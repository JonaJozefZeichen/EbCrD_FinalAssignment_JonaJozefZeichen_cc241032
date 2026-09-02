using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    // Identifies what destroyed the object so the matching feedback effect can be shown
    public enum DestructionCause
    {
        LaserHit,
        PlanetImpact
    }

    [Header("Destruction Settings")]
    [SerializeField] private GameObject laserHitEffectPrefab; // Spawns when destroyed by a laser bullet
    [SerializeField] private GameObject planetImpactEffectPrefab; // Spawns when destroyed by crashing into the planet

    private Collider planetCollider; // Assigned externally (see MeteoriteSpawner) - avoids depending on where PlanetGravitySource happens to sit in the scene hierarchy

    private void OnCollisionEnter(Collision collision)
    {
        // Compared by reference rather than looked up via component search, since the planet's
        // SphereCollider and its gravity script live on two different GameObjects in the scene
        if (planetCollider != null && collision.collider == planetCollider)
        {
            DestroyTarget(DestructionCause.PlanetImpact);
        }
    }

    public void SetPlanetCollider(Collider collider)
    {
        planetCollider = collider;
    }

    public void DestroyTarget(DestructionCause cause)
    {
        // Pick the feedback effect matching how the object was destroyed
        GameObject effectPrefab = cause == DestructionCause.LaserHit ? laserHitEffectPrefab : planetImpactEffectPrefab;

        // Spawn particle visual effect at target location if assigned
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, transform.rotation);
        }

        // Remove target object from scene hierarchy
        Destroy(gameObject);
    }
}