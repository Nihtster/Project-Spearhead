using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 400f;           // Movement speed
    public float jumpForce = 50f;            // Jump strength
    public float drag = 10f;                 // How quickly the player stops when no input
    private Vector3 moveDirection;           // Stores the player's movement direction
    private CharacterController playerController; // Reference to the CharacterController

    private Vector3 startPosition;           // Starting position for respawning

    public float gravity = -9.81f;           // Gravity force
    public float jumpHeight = 50f;           // Height for the jump
    private float velocityY;                 // Store vertical velocity (jump/gravity)

    void Start()
    {
        gravity *= 1000;
        playerController = GetComponent<CharacterController>(); // Get the CharacterController
        startPosition = transform.position; // Store the starting position
    }

    void Update()
    {
        // Handle jumping and gravity
        if (playerController.isGrounded)
        {
            // If the player is grounded, reset the Y velocity and allow jumping
            velocityY = -1f; // Small downward force to keep player grounded
            if (Input.GetKeyDown(KeyCode.Space)) // Jump on spacebar press
            {
                velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity); // Apply jump force
            }
        }
        else
        {
            // If not grounded, apply gravity to the Y velocity
            velocityY += gravity * Time.deltaTime;
        }

        // Handle player movement
        HandleMovement();
    }

    void FixedUpdate()
    {
        // Update moveDirection with the vertical velocity
        moveDirection.y = velocityY;

        // Apply movement using CharacterController
        playerController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // Get movement input (WASD or arrow keys)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to player's forward
        moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection = moveDirection.normalized * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle respawn logic
        if (collision.gameObject.CompareTag("Reset"))
        {
            Debug.Log("Respawn box triggered");
            transform.position = startPosition;
            transform.rotation = Quaternion.identity;
        }
    }
}
