using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DestructibleObject))]
public class MeteoriteMovement : MonoBehaviour
{
    [Header("Flight & Attraction")]
    [SerializeField] private float fallSpeed = 6.0f;
    [SerializeField] private float rotationSpeed = 30.0f;

    private Rigidbody rb;
    private Transform gravityTarget;
    private Vector3 tumbleAxis;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

        rb.MovePosition(rb.position + toCenter.normalized * fallSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(tumbleAxis * rotationSpeed * Time.fixedDeltaTime));
    }

    public void SetGravityTarget(Transform target)
    {
        gravityTarget = target;
    }
}
