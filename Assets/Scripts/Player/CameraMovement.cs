using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;  // Reference to the FreeLook Camera (Cinemachine)
    public Camera followCamera;  // Reference to the regular Unity Camera (Non-Cinemachine)

    public KeyCode toggleKey = KeyCode.C;  // Key to switch between cameras (hold key)

    private bool isFreeLookActive = false;  // To track if the FreeLook camera is active

    // Define the default position and rotation for the FreeLook camera
    public Vector3 defaultFreeLookPosition = new Vector3(0f, 5f, -10f);  // Example default position
    public Quaternion defaultFreeLookRotation = Quaternion.Euler(30f, 0f, 0f);  // Example default rotation

    void Start()
    {
        // Ensure one camera is active at the start
        SwitchToFollowCamera();  // Default camera is the Follow camera (Non-Cinemachine)
    }

    void Update()
    {
        // Handle camera switching with a key press
        if (Input.GetKey(toggleKey))
        {
            // Switch to FreeLook camera if it's not already active
            if (!isFreeLookActive)
            {
                SwitchToFreeLookCamera();
            }
        }
        else
        {
            // If the key is not held, revert back to the follow camera and reset FreeLook camera
            if (isFreeLookActive)
            {
                SwitchToFollowCamera();
                ResetFreeLookCamera(); // Reset FreeLook camera to predefined position and rotation
            }
        }
    }

    private void ResetFreeLookCamera()
    {
        // Reset the FreeLook camera's position and rotation to the predefined default values
        freeLookCamera.transform.position = defaultFreeLookPosition;
        freeLookCamera.transform.rotation = defaultFreeLookRotation;
    }

    private void SwitchToFollowCamera()
    {
        // Deactivate FreeLook camera and activate the regular Unity camera
        freeLookCamera.gameObject.SetActive(false);
        followCamera.gameObject.SetActive(true);
        isFreeLookActive = false;
    }

    private void SwitchToFreeLookCamera()
    {
        // Reset the FreeLook camera to the predefined default position and rotation
        ResetFreeLookCamera();

        // Deactivate regular Unity camera and activate FreeLook camera
        followCamera.gameObject.SetActive(false);
        freeLookCamera.gameObject.SetActive(true);
        isFreeLookActive = true;
    }
}
