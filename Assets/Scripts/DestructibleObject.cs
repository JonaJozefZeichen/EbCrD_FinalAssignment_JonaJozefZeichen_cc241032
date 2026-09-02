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