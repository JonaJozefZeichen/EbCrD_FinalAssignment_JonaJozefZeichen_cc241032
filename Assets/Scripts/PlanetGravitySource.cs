using UnityEngine;

public class PlanetGravitySource : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 25.0f;
    [SerializeField] private float alignmentSpeed = 50.0f;

    public void Attract(Rigidbody body)
    {
        if (body == null) return;

        Vector3 surfaceNormal = (body.position - transform.position).normalized;
        Vector3 gravityDirection = -surfaceNormal;

        body.AddForce(gravityDirection * gravity, ForceMode.Acceleration);

        Vector3 currentForward = body.transform.forward;
        Vector3 alignedForward = Vector3.ProjectOnPlane(currentForward, surfaceNormal).normalized;

        if (alignedForward.sqrMagnitude < 0.001f)
        {
            alignedForward = Vector3.ProjectOnPlane(body.transform.up, surfaceNormal).normalized;
        }

        Quaternion targetRotation = Quaternion.LookRotation(alignedForward, surfaceNormal);

        body.MoveRotation(Quaternion.Slerp(body.rotation, targetRotation, alignmentSpeed * Time.fixedDeltaTime));
    }
}
