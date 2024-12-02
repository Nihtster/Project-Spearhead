using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float jumpForce = 5f; // Jump strength
    public float rotationSpeed = 100f; // Rotation speed for the camera
    public float drag = 10f; // How quickly the player stops when no input
    private Vector3 moveDirection; // Stores the player's movement direction
    private Rigidbody playerRB; // Reference to the player's Rigidbody
    private bool isGrounded = true; // Whether the player is grounded
    private Vector3 startPosition; // Starting position for respawning

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.drag = drag; // Apply drag directly to Rigidbody for smooth stopping
        startPosition = transform.position; // Store starting position
    }

    void Update()
    {
        // Handle rotation based on mouse movement
        float mouseRotate = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, mouseRotate, 0);

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRB.velocity = new Vector3(playerRB.velocity.x, jumpForce, playerRB.velocity.z);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // Get movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to player's forward
        moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection = moveDirection.normalized * moveSpeed;

        // Apply velocity directly to Rigidbody
        Vector3 velocity = new Vector3(moveDirection.x, playerRB.velocity.y, moveDirection.z);
        playerRB.velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if player is grounded
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("World"))
        {
            isGrounded = true;
        }

        // Handle respawn logic
        if (collision.gameObject.CompareTag("Reset"))
        {
            Debug.Log("Respawn box triggered");
            playerRB.velocity = Vector3.zero;
            transform.position = startPosition;
            transform.rotation = Quaternion.identity;
        }
    }
}
