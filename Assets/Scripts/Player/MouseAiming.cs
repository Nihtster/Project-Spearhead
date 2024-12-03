using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAiming : MonoBehaviour
{
    public float rotationSpeed = 5f;       // Speed of rotation for smooth aiming.
    public float minVerticalAngle = -60f; // Minimum vertical rotation angle (looking down).
    public float maxVerticalAngle = 60f;  // Maximum vertical rotation angle (looking up).

    private float verticalRotation = 0f;  // Tracks the current vertical rotation angle.
    private Camera playerCamera;  // The camera that will rotate independently.

    void Start()
    {
        // Lock and hide the cursor to keep it within the game window.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize the camera reference.
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            // Allow camera to rotate independently when "C" key is pressed.
            HandleCameraRotation();
        }
        else
        {
            // Apply body rotation (as usual) when "C" key is released.
            HandleBodyRotation();
        }

        // Unlock the cursor when pressing Escape.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleCameraRotation()
    {
        // Get mouse movement input for free camera rotation.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate the camera independently based on mouse movement.
        playerCamera.transform.Rotate(Vector3.up * mouseX * rotationSpeed, Space.World);
        verticalRotation -= mouseY * rotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // Apply the vertical rotation to the camera.
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, playerCamera.transform.localRotation.eulerAngles.y, 0f);
    }

    private void HandleBodyRotation()
    {
        // Get mouse movement input for body rotation.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate the body horizontally based on mouse X movement.
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);

        // Update the vertical rotation of the mech's body.
        verticalRotation -= mouseY * rotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // Apply the vertical rotation to the mech's body.
        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localRotation.eulerAngles.y, 0f);
    }
}
