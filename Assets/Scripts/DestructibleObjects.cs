using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Destruction Settings")]
    [SerializeField] private GameObject breakEffectPrefab; // Spawns debris or explosion particle upon destruction

    public void DestroyTarget()
    {
        // Spawn particle visual effect at target location if assigned
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, transform.rotation);
        }

        // Remove target object from scene hierarchy
        Destroy(gameObject);
    }
}