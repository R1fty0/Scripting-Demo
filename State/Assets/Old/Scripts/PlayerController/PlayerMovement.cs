// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

    #region Public Variables 

    [Header("References")]
    [Tooltip("The script MUST have these filled in order to work!")]
    public Transform playerCam;
    public Transform orientation;
    public LayerMask ground;

    [Header("Camera Settings")]
    public float sensitivity = 50f;
    public float sensMultiplier = 1f;

    [Header("Accelerative Forces")]
    public float moveForce = 4500;
    public float slideForce = 400;
    public float jumpForce = 550f;
    public float gravity = 20.0f;

    [Header("Restrictive Forces")]
    public float frictionForce = 0.175f;
    public float frictionWhileSliding = 0.2f;
    
    [Header("Limits")]
    public float maxSpeed = 20;
    public float maxSlopeAngle = 35f;

    [Header("Misc")]
    public float jumpCooldown = 0.25f;
    // I don't really understand what this variable does.
    public float frictionThreshold = 0.01f;

    #endregion 

    #region Private Variables
    private Rigidbody rb;
    private float xRotation;
    private bool isGrounded;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    private bool readyToJump = true;
    float x, y;
    bool jumping, sprinting, crouching;
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    #endregion

    #region Execution Functions 

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        Input();
        Look();
    }

    #endregion

    #region Input

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void Input() {
        x = UnityEngine.Input.GetAxisRaw("Horizontal");
        y = UnityEngine.Input.GetAxisRaw("Vertical");
        jumping = UnityEngine.Input.GetButton("Jump");
        crouching = UnityEngine.Input.GetKey(KeyCode.LeftControl);
      
        //Crouching
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (UnityEngine.Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    #endregion

    #region Crouching
    private void StartCrouch() {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f) {
            if (isGrounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
    #endregion

    private void Movement() {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * gravity);
        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;
        
        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && isGrounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!isGrounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        
        // Movement while sliding
        if (isGrounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveForce * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveForce * Time.deltaTime * multiplier);
    }

    #region Jumping
    private void Jump() {
        if (isGrounded && readyToJump) {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
    }

    #endregion

    private float desiredX;
    private void Look() {
        float mouseX = UnityEngine.Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = UnityEngine.Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!isGrounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveForce * Time.deltaTime * -rb.velocity.normalized * frictionWhileSliding);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > frictionThreshold && Math.Abs(x) < 0.05f || (mag.x < -frictionThreshold && x > 0) || (mag.x > frictionThreshold && x < 0)) {
            rb.AddForce(moveForce * orientation.transform.right * Time.deltaTime * -mag.x * frictionForce);
        }
        if (Math.Abs(mag.y) > frictionThreshold && Math.Abs(y) < 0.05f || (mag.y < -frictionThreshold && y > 0) || (mag.y > frictionThreshold && y < 0)) {
            rb.AddForce(moveForce * orientation.transform.forward * Time.deltaTime * -mag.y * frictionForce);
        }
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking.
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    
    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (ground != (ground | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                isGrounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        isGrounded = false;
    }
}
