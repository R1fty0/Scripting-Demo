using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    // To-Do: Modify player controller to follow platform beneath player if it is moving 

    [Header("Settings")]
    [SerializeField] private float speed;
    [SerializeField] private ControlValue controlValue;
    [SerializeField] private MovementAxis movementAxis;
    [SerializeField] private InitalMovementDirection initalMovementDirection;

    [Header("Time Based Movement")]
    [Tooltip("Only edit these values if the control value is set to time.")]
    [SerializeField] private float timeTillDirectionChange;

    private Vector3 _currentMovementDirection;
    private MotionState _currentMotionState;

    enum InitalMovementDirection
    {
        Up, 
        Down,
        Left, 
        Right,
        Forward,
        Back 
    }

    enum ControlValue
    {
        Time,
        Distance,
        Collision
    }

    enum MotionState
    {
        Positive,
        Negative, 
        None 
    }

    enum MovementAxis
    {
        X,
        Y,
        Z
    }

    
    void Start()
    {
        _currentMotionState = MotionState.None;
        DetermineStartingMovementDirection();
    }

    void DetermineStartingMovementDirection()
    {
        switch (initalMovementDirection)
        {
            case InitalMovementDirection.Up:
                _currentMovementDirection = Vector3.up;
                break; 
            case InitalMovementDirection.Down:
                _currentMovementDirection = Vector3.down;
                break;
            case InitalMovementDirection.Left:
                _currentMovementDirection = Vector3.left;
                break;
            case InitalMovementDirection.Right:
                _currentMovementDirection = Vector3.right;
                break;
            case InitalMovementDirection.Forward:
                _currentMovementDirection = Vector3.forward;
                break;
            case InitalMovementDirection.Back:
                _currentMovementDirection = Vector3.back;
                break;
        }
    }

    
    void Update()
    {
        Move(_currentMovementDirection, speed);
    }

    void Move(Vector3 direction, float speed)
    {
        // Calculate the movement vector 
        Vector3 movementVector = direction * speed;
        // Apply movement 
        transform.position += movementVector * Time.deltaTime;
    }
}
