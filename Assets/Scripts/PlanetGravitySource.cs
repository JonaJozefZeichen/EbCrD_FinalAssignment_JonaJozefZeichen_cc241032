using UnityEngine;

public class PlanetGravitySource : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 25.0f; // Sets inward gravitational acceleration force
    [SerializeField] private float alignmentSpeed = 50.0f; // Controls how smoothly body reorients feet toward center

    public void Attract(Rigidbody body)
    {
        // Skip processing if reference is missing to avoid crashes
        if (body == null) return;

        // Calculate unit normal vector pointing straight outward from planet core
        Vector3 surfaceNormal = (body.position - transform.position).normalized;
        Vector3 gravityDirection = -surfaceNormal;

        // Apply constant inward acceleration pulling player toward planet core
        body.AddForce(gravityDirection * gravity, ForceMode.Acceleration);

        // Calculate target forward vector perpendicular to surface normal to preserve yaw orientation
        Vector3 currentForward = body.transform.forward;
        Vector3 alignedForward = Vector3.ProjectOnPlane(currentForward, surfaceNormal).normalized;

        // Fall back to current forward if vector drops to zero to prevent invalid rotation
        if (alignedForward.sqrMagnitude < 0.001f)
        {
            alignedForward = Vector3.ProjectOnPlane(body.transform.up, surfaceNormal).normalized;
        }

        // Create target rotation where local Up matches surface normal and local Forward matches surface tangent
        Quaternion targetRotation = Quaternion.LookRotation(alignedForward, surfaceNormal);

        // Smoothly rotate rigidbody through physics engine
        body.MoveRotation(Quaternion.Slerp(body.rotation, targetRotation, alignmentSpeed * Time.fixedDeltaTime));
    }
}