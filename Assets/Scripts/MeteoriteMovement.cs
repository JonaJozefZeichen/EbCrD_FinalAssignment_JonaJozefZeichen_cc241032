using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DestructibleObject))]
public class MeteoriteMovement : MonoBehaviour
{
    [Header("Flight & Attraction")]
    [SerializeField] private float fallSpeed = 6.0f; // Sets baseline inward travel velocity toward planet center
    [SerializeField] private float rotationSpeed = 30.0f; // Adds tumbling rotation so meteorites feel organic during flight

    [Header("Impact Detection")]
    [SerializeField] private float impactDistance = 10.75f; // Planet radius (10) + meteorite radius (0.75), update if the planet's scale changes

    private Rigidbody rb; // Kinematic, moving a collider with no Rigidbody every frame is expensive in PhysX
    private DestructibleObject destructible;
    private Transform gravityTarget;
    private Vector3 tumbleAxis;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        destructible = GetComponent<DestructibleObject>();

        // Everything here is driven by hand through MovePosition, physics forces would fight it
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Start()
    {
        tumbleAxis = Random.insideUnitSphere.normalized;
    }

    private void FixedUpdate()
    {
        if (gravityTarget == null) return;

        Vector3 toCenter = gravityTarget.position - transform.position;

        // Checked by distance rather than OnCollisionEnter - a kinematic Rigidbody against a
        // Planet with no Rigidbody of its own doesn't reliably generate contacts in PhysX
        if (toCenter.sqrMagnitude <= impactDistance * impactDistance)
        {
            // Bail out here so a second FixedUpdate this frame (possible on a slow frame) can't double-fire this
            enabled = false;
            destructible.DestroyTarget(DestructibleObject.DestructionCause.PlanetImpact);
            return;
        }

        rb.MovePosition(rb.position + toCenter.normalized * fallSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(tumbleAxis * rotationSpeed * Time.fixedDeltaTime));
    }

    public void SetGravityTarget(Transform target)
    {
        gravityTarget = target;
    }
}