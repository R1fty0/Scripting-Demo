using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TPPMovement : MonoBehaviour
{
    private CharacterController m_CharacterController;

    [Header("Settings")]
    public float speed = 5.0f;
    public float turnSmoothTime = 0.1f;

    [Header("References")]
    public Transform actualCamera; 

    private float turnSmoothVelocity; 

    
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        Vector3 inputVector = _Input();
        if (inputVector.magnitude >= 0.1f)
        {
            // Rotate the player, get a movement vector
            Vector3 movementVector = RotatePlayer(inputVector);
            // Move the player
            Move(movementVector);
        }
    }

    /// <summary>
    /// Rotates the player based on a given input vector, calculates and returns a movement vector. 
    /// </summary>
    /// <param name="inputVector">Vector returned by _Input() function</param>
    /// <returns></returns>
    Vector3 RotatePlayer(Vector3 inputVector)
    {
        float targetAngle = Mathf.Atan2(inputVector.x, inputVector.z) * Mathf.Rad2Deg + actualCamera.eulerAngles.y;
        float currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, currentAngle, 0.0f);
        Vector3 movementVector = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
        return movementVector;
    }

    /// <summary>
    /// Move the player given a movement vector. 
    /// </summary>
    /// <param name="movementVector"></param>
    void Move(Vector3 movementVector)
    {
        if (movementVector.magnitude >= 0.1f)
        {
            m_CharacterController.Move(movementVector.normalized * speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Get input from the player and return a movement vector. 
    /// </summary>
    /// <returns></returns>
    Vector3 _Input()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        return direction; 
    }
}