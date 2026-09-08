using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public enum DestructionCause
    {
        LaserHit,
        PlanetImpact
    }

    [Header("Destruction Settings")]
    [SerializeField] private GameObject laserHitEffectPrefab;
    [SerializeField] private GameObject planetImpactEffectPrefab;
    [SerializeField] private int planetImpactDamage = 10;

    private Collider planetCollider;

    private void OnCollisionEnter(Collision collision)
    {
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
        GameObject effectPrefab = cause == DestructionCause.LaserHit ? laserHitEffectPrefab : planetImpactEffectPrefab;

        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, transform.rotation);
        }

        if (cause == DestructionCause.LaserHit && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterMeteoriteDestroyed();
        }

        if (cause == DestructionCause.PlanetImpact && PlanetHealth.Instance != null)
        {
            PlanetHealth.Instance.TakeDamage(planetImpactDamage);
        }

        Destroy(gameObject);
    }
}
