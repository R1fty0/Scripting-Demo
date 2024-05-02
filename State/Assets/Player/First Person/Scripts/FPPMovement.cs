using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class FPPMovement : MonoBehaviour
{
    [Header("Settings")]
    public float speed;
    public float gravity = -9.81f;
    public float jumpHeight;
    public KeyCode jumpButton; 

    [Header("References")]
    [Tooltip("The lowest point of the player.")]
    public Transform groundCheck;
    [Tooltip("Layer containing all ground objects.")]
    public LayerMask groundMask;

    #region Private Variables
    CharacterController controller;
    Vector3 currentVelocity;
    bool isGrounded;

    #endregion

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Movement();
        Jumping();
    }

    void Movement()
    {
        // Get movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction 
        Vector3 movementDirection = transform.right * x + transform.forward * z;

        // Move
        controller.Move(movementDirection * speed * Time.deltaTime);
    }

    // Doesn't work if VSync is not enabled 
    void Jumping()
    {
        // Check if player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);
        if (Input.GetKeyDown(jumpButton) && isGrounded)
        {
            currentVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        else if (!isGrounded)
        {
            currentVelocity.y += gravity * Time.deltaTime;
            
        }
        else if (isGrounded && currentVelocity.y > 0.0f)
        {
            // Reset gravity
            currentVelocity.y = -2.0f;
        }

        controller.Move(currentVelocity * Time.deltaTime);
    }
}
