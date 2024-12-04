using System.Collections;
using UnityEngine;

public class MechWalker : MonoBehaviour
{
    [Header("Leg Settings")]
    public Transform[] legTargets; // Assign leg IK targets
    public Transform[] legHints;   // Assign hint objects (root of legs)
    public float stepHeight = 0.5f; // How high the legs lift while stepping
    public float stepDuration = 0.25f; // How long a step takes
    public float strideLength = 2.0f; // Distance before a leg takes a step

    [Header("Ground Detection")]
    public LayerMask groundLayer; // Layer mask for the ground
    public float raycastDistance = 5f; // Maximum raycast distance
    public float stepHeightOffset = 0.1f; // Offset above ground to prevent clipping

    private Vector3[] defaultPositions; // Default positions relative to the hint objects
    private Vector3[] currentTargets; // Current targets for each leg
    private Vector3[] predictedTargets; // Predicted next step positions
    private bool[] legInMotion; // Track whether each leg is moving

    private Vector3 previousPosition; // For manual velocity calculation
    private int[] stepOrder = new int[] { 0, 3, 1, 2 }; // Gait sequence (trot)

    public void Start()
    {
        // Initialize arrays
        defaultPositions = new Vector3[legTargets.Length];
        currentTargets = new Vector3[legTargets.Length];
        predictedTargets = new Vector3[legTargets.Length];
        legInMotion = new bool[legTargets.Length];
        previousPosition = transform.position;

        // Store default positions relative to each leg's hint
        for (int i = 0; i < legTargets.Length; i++)
        {
            defaultPositions[i] = legHints[i].InverseTransformPoint(legTargets[i].position);
            currentTargets[i] = legTargets[i].position;
        }
    }

    public void Update()
    {
        // Calculate velocity based on position change
        Vector3 mechVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        // Process legs in the defined step order for a trotting gait
        foreach (int legIndex in stepOrder)
        {
            if (!legInMotion[legIndex])
            {
                Vector3 targetPosition = CalculateTargetPosition(legIndex, mechVelocity);

                // Store predicted target for visualization
                predictedTargets[legIndex] = targetPosition;

                // If the target position is far enough, initiate a step
                if (Vector3.Distance(currentTargets[legIndex], targetPosition) > strideLength)
                {
                    StartCoroutine(MoveLeg(legIndex, targetPosition));
                }
            }
        }
    }

    Vector3 CalculateTargetPosition(int legIndex, Vector3 mechVelocity)
    {
        // Calculate the desired position relative to the leg's hint object
        Vector3 hintWorldPosition = legHints[legIndex].TransformPoint(defaultPositions[legIndex]);
        Vector3 forwardOffset = mechVelocity.normalized * strideLength / 2; // Predict forward placement
        Vector3 targetPosition = hintWorldPosition + forwardOffset;

        // Use raycasting to adjust for ground height
        Ray ray = new Ray(targetPosition + Vector3.up * raycastDistance, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance * 2, groundLayer))
        {
            targetPosition = hit.point + Vector3.up * stepHeightOffset; // Adjust for ground height
        }

        return targetPosition;
    }

    IEnumerator MoveLeg(int legIndex, Vector3 targetPosition)
    {
        legInMotion[legIndex] = true;

        Vector3 startPosition = legTargets[legIndex].position;
        float elapsed = 0;

        while (elapsed < stepDuration)
        {
            // Interpolate position with time
            float t = elapsed / stepDuration;
            Vector3 interpolatedPosition = Vector3.Lerp(startPosition, targetPosition, t);

            // Add an arc to the motion for stepping
            interpolatedPosition.y += Mathf.Sin(t * Mathf.PI) * stepHeight;

            // Raycast to adjust the foot height
            Ray ray = new Ray(interpolatedPosition + Vector3.up * raycastDistance, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance * 2, groundLayer))
            {
                // Adjust position to the ground
                interpolatedPosition = hit.point + Vector3.up * stepHeightOffset;
            }

            // Update the IK target position
            legTargets[legIndex].position = interpolatedPosition;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Snap the leg to the target position at the end of the step
        legTargets[legIndex].position = targetPosition;
        currentTargets[legIndex] = targetPosition;

        legInMotion[legIndex] = false;

        // Introduce a small delay between steps to synchronize the gait
        yield return new WaitForSeconds(0.05f);
    }

    // Visualize predicted targets for debugging
    private void OnDrawGizmos()
    {
        if (predictedTargets == null) return;

        Gizmos.color = Color.red;
        foreach (Vector3 predictedTarget in predictedTargets)
        {
            Gizmos.DrawSphere(predictedTarget, 0.2f);
        }
    }
}
