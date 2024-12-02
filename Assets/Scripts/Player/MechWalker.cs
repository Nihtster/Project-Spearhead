using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechWalker : MonoBehaviour
{
    [Header("Leg Settings")]
    public Transform[] legTargets; // Assign leg IK targets
    public Transform[] legHints;   // Assign hint objects (root of legs)
    public float stepHeight = 0.5f; // How high the legs lift while stepping
    public float stepDuration = 0.25f; // How long a step takes
    public float strideLength = 2.0f; // Distance before a leg takes a step

    private Vector3[] defaultPositions; // Default positions relative to the hint objects
    private Vector3[] currentTargets; // Current targets for each leg
    private bool[] legInMotion; // Track whether each leg is moving

    // Define step order for a trot gait (diagonal pairs)
    private int[] stepOrder = new int[] { 0, 3, 1, 2 }; // 0 = Front Left, 3 = Back Right, 1 = Front Right, 2 = Back Left

    void Start()
    {
        // Initialize arrays
        defaultPositions = new Vector3[legTargets.Length];
        currentTargets = new Vector3[legTargets.Length];
        legInMotion = new bool[legTargets.Length];

        // Calculate and store default positions relative to each leg's hint
        for (int i = 0; i < legTargets.Length; i++)
        {
            defaultPositions[i] = legHints[i].InverseTransformPoint(legTargets[i].position);
            currentTargets[i] = legTargets[i].position;
        }
    }

    void Update()
    {
        // Process legs in the defined step order for alternating gait
        foreach (int legIndex in stepOrder)
        {
            if (!legInMotion[legIndex])
            {
                // Calculate the target position for the leg relative to its hint
                Vector3 targetPosition = CalculateTargetPosition(legIndex);

                // If the target position is far enough from the current position, initiate a step
                if (Vector3.Distance(targetPosition, currentTargets[legIndex]) > strideLength)
                {
                    StartCoroutine(MoveLeg(legIndex, targetPosition));
                }
            }
        }
    }

    Vector3 CalculateTargetPosition(int legIndex)
    {
        // Calculate the desired position relative to the leg's hint object
        Vector3 hintWorldPosition = legHints[legIndex].TransformPoint(defaultPositions[legIndex]);
        Vector3 forwardOffset = transform.forward * strideLength / 2; // Adjust stride length based on movement direction
        return hintWorldPosition + forwardOffset;
    }

    System.Collections.IEnumerator MoveLeg(int legIndex, Vector3 targetPosition)
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

            // Update the IK target position
            legTargets[legIndex].position = interpolatedPosition;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap the leg to the target position at the end of the step
        legTargets[legIndex].position = targetPosition;
        currentTargets[legIndex] = targetPosition;

        legInMotion[legIndex] = false;

        // Introduce a small delay between steps to synchronize the gait
        yield return new WaitForSeconds(0.05f);
    }
}
