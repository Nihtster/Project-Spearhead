using System.Collections;
using UnityEngine;

public class RaycastAiming : MonoBehaviour
{
    public Transform grappleHook1; // First grapple hook transform (left or primary hook)
    public Transform grappleHook2; // Second grapple hook transform (right or secondary hook)
    public float grappleMaxRange = 50f; // Maximum range for the grapple hook raycasts
    public float cannonMaxRange = 100f; // Maximum range for the cannon raycast
    public GameObject sphereIndicatorPrefab; // Prefab for the red sphere indicator for grapple hooks
    public GameObject cannonIndicatorPrefab; // Prefab for the sphere indicator for the cannon
    public KeyCode moveToTopKey = KeyCode.E; // Key to toggle movement towards the target
    public GameObject modelToMove; // The model that will move to the target when toggled
    public float moveSpeed = 5f; // Speed at which the mech moves towards the target
    public Vector3 landingOffset = new Vector3(0f, 50f, 0f); // Offset applied to the landing position
    public float raycastSeparation = 1f; // Distance between the two raycast origins
    public Transform cannonFiringPoint; // Cannon's firing point transform
    private GameObject sphereIndicatorCannon; // Instance of the cannon sphere indicator

    private CharacterController characterController; // Reference to the character controller
    private GameObject sphereIndicator1; // Instance of the first sphere indicator
    private GameObject sphereIndicator2; // Instance of the second sphere indicator
    private bool isMoving = false; // Tracks whether the mech is moving towards a target
    private Vector3 targetPosition; // Target position for smooth movement

    void Start()
    {
        // Get the CharacterController component
        characterController = modelToMove.GetComponent<CharacterController>();

        // Instantiate the two sphere indicators for the grapple hooks and cannon, and deactivate them initially
        sphereIndicator1 = Instantiate(sphereIndicatorPrefab);
        sphereIndicator2 = Instantiate(sphereIndicatorPrefab);
        sphereIndicatorCannon = Instantiate(cannonIndicatorPrefab); // Using the cannon indicator prefab

        sphereIndicator1.SetActive(false);
        sphereIndicator2.SetActive(false);
        sphereIndicatorCannon.SetActive(false);
    }

    void Update()
    {
        // Cast rays from the two grapple hook points with separate ranges
        RaycastHit hit1, hit2;
        bool hitHook1 = Physics.Raycast(grappleHook1.position, grappleHook1.forward, out hit1, grappleMaxRange);
        bool hitHook2 = Physics.Raycast(grappleHook2.position, grappleHook2.forward, out hit2, grappleMaxRange);

        // Cast a ray from the cannon's firing point with a different range
        RaycastHit hitCannon;
        bool hitCannonRay = Physics.Raycast(cannonFiringPoint.position, cannonFiringPoint.forward, out hitCannon, cannonMaxRange);

        if (hitHook1 && hitHook2)
        {
            // Position the sphere indicators at the respective hit points
            sphereIndicator1.transform.position = hit1.point;
            sphereIndicator2.transform.position = hit2.point;

            sphereIndicator1.SetActive(true);
            sphereIndicator2.SetActive(true);

            // Calculate the target position as the average of both hits
            targetPosition = (hit1.point + hit2.point) / 2 + landingOffset;
        }
        else
        {
            // If no valid hits, disable both sphere indicators
            sphereIndicator1.SetActive(false);
            sphereIndicator2.SetActive(false);
        }

        // If the cannon ray hits something, position the cannon sphere indicator
        if (hitCannonRay)
        {
            sphereIndicatorCannon.transform.position = hitCannon.point;
            sphereIndicatorCannon.SetActive(true);

            // Debug output for cannon hit
            Debug.Log("Cannon hit: " + hitCannon.collider.name);
        }
        else
        {
            sphereIndicatorCannon.SetActive(false);
        }

        // Toggle movement towards the target when pressing the key
        if (sphereIndicator1.activeSelf && sphereIndicator2.activeSelf && Input.GetKeyDown(moveToTopKey))
        {
            isMoving = !isMoving; // Toggle movement state

            // Disable character controller during manual movement
            if (characterController != null && isMoving)
            {
                characterController.enabled = false;
            }
        }

        // Handle movement towards the target
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        // Smoothly move the mech to the target position
        modelToMove.transform.position = Vector3.MoveTowards(
            modelToMove.transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Check if the mech has reached the target
        if (Vector3.Distance(modelToMove.transform.position, targetPosition) < 0.1f)
        {
            isMoving = false; // Stop moving

            // Re-enable the character controller
            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grappleHook1 != null && grappleHook2 != null)
        {
            Gizmos.color = Color.red;

            // Draw two rays for debugging from each grapple hook with the grapple range
            Gizmos.DrawRay(grappleHook1.position, grappleHook1.forward * grappleMaxRange);
            Gizmos.DrawRay(grappleHook2.position, grappleHook2.forward * grappleMaxRange);
        }

        // Draw the cannon's ray for debugging with the cannon range
        if (cannonFiringPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(cannonFiringPoint.position, cannonFiringPoint.forward * cannonMaxRange);
        }
    }
}
