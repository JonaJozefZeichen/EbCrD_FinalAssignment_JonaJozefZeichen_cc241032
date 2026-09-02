using UnityEngine;

public class PlanetGravitySource : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 25.0f; // Sets inward gravitational acceleration force
    [SerializeField] private float alignmentSpeed = 50.0f; // Controls how smoothly body reorients feet toward center

    public void Attract(Rigidbody body)
    {
        if (body == null) return;

        // "Down" is whatever direction points away from the core, not a fixed world axis
        Vector3 surfaceNormal = (body.position - transform.position).normalized;
        Vector3 gravityDirection = -surfaceNormal;

        // ForceMode.Acceleration ignores mass, so everything falls at the same rate regardless of Rigidbody.mass
        body.AddForce(gravityDirection * gravity, ForceMode.Acceleration);

        // Keep the body's current facing but flatten it onto the new tangent plane, otherwise yaw resets every frame
        Vector3 currentForward = body.transform.forward;
        Vector3 alignedForward = Vector3.ProjectOnPlane(currentForward, surfaceNormal).normalized;

        // Forward and the surface normal can end up parallel (e.g. looking straight up), which zeroes the projection above
        if (alignedForward.sqrMagnitude < 0.001f)
        {
            alignedForward = Vector3.ProjectOnPlane(body.transform.up, surfaceNormal).normalized;
        }

        Quaternion targetRotation = Quaternion.LookRotation(alignedForward, surfaceNormal);

        // Slerp rather than snap, otherwise crossing terrain seams causes visible rotation pops
        body.MoveRotation(Quaternion.Slerp(body.rotation, targetRotation, alignmentSpeed * Time.fixedDeltaTime));
    }
}