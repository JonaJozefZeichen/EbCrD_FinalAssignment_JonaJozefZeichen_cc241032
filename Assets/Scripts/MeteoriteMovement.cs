using UnityEngine;

[RequireComponent(typeof(DestructibleObject))]
public class MeteoriteMovement : MonoBehaviour
{
    [Header("Flight & Attraction")]
    [SerializeField] private float fallSpeed = 6.0f; // Sets baseline inward travel velocity toward planet center
    [SerializeField] private float rotationSpeed = 30.0f; // Adds tumbling rotation so meteorites feel organic during flight

    private Transform gravityTarget; // Tracks planet center transform to pull meteorite inward
    private Vector3 tumbleAxis; // Stores randomized rotational axis for natural tumbling

    private void Start()
    {
        // Pick random axis to apply constant rotational spin
        tumbleAxis = Random.insideUnitSphere.normalized;
    }

    private void Update()
    {
        // Skip movement if planet reference is lost or missing
        if (gravityTarget == null) return;

        // Calculate direction vector pointing straight toward planet center
        Vector3 pullDirection = (gravityTarget.position - transform.position).normalized;

        // Move meteorite inward toward target position
        transform.position += pullDirection * fallSpeed * Time.deltaTime;

        // Apply tumbling rotation around random axis
        transform.Rotate(tumbleAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }

    public void SetGravityTarget(Transform target)
    {
        // Assign target transform from spawner
        gravityTarget = target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Trigger destruction if meteorite crashes into planet ground surface
        DestructibleObject destructible = GetComponent<DestructibleObject>();
        if (destructible != null)
        {
            destructible.DestroyTarget();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}