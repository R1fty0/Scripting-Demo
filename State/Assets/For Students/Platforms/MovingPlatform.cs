using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // To-Do: Modify player controller to follow platform beneath player if it is moving 
    // To-Do: Prevent the inital movement direction from not being on the axis of movement 
    // To-Do: Resolve weird bug with distance-based platform movement. 

    #region Public Variables
    [Tooltip("How fast the platform will travel. ")]
    [SerializeField] private float speed = 3.0f;

    [Header("Direction Settings")]
    [Tooltip("The direction the platform will travel when the game starts.")]
    [SerializeField] private MovementDirection initalMovementDirection;
    [Tooltip("The axis which the platform will move along: Up & Down = Y, Forward & Backwards = Z, Right & Left = X")]
    [SerializeField] private MovementAxis movementAxis;
    

    [Header("Trigger Settings")]
    [Tooltip("What attribute the script will use to determine when to change direction.")]
    [SerializeField] private Control control;
    [Tooltip("How long (time in seconds) the platform will travel before changing direction.")]
    [SerializeField] private float timeTillSwitch;
    [Tooltip("How far (distance in metres) the platform will travel before changing direction.")]
    [Range(0.0f, float.MaxValue)] 
    [SerializeField] private float distanceTillSwitch;

    #endregion

    #region Private Variables

    private MovementDirection _currentMovementDirection;
    private MovementDirection _positiveMovementDirection;
    private MovementDirection _negativeMovementDirection;

    private Vector3 _initalPosition;
    private float _timeSinceLastSwitch = 0.0f;

    #endregion

    #region Enums
    // Determines what will be measured in order to trigger a change in direction 
    enum Control
    {
        Time,
        Distance
    }

    enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }

    enum MovementAxis
    {
        X, 
        Y,
        Z
    }

    #endregion

    #region Setup
    void Start()
    {
        AxisSetup();
        _currentMovementDirection = initalMovementDirection;
        _initalPosition = transform.position;
    }

    void AxisSetup()
    {
        if (movementAxis == MovementAxis.X)
        {
            _positiveMovementDirection = MovementDirection.Left;
            _negativeMovementDirection = MovementDirection.Right;
        }
        else if (movementAxis == MovementAxis.Y)
        {
            _positiveMovementDirection = MovementDirection.Up;
            _negativeMovementDirection = MovementDirection.Down;
        }
        else if (movementAxis == MovementAxis.Z)
        {
            _positiveMovementDirection = MovementDirection.Forward;
            _negativeMovementDirection = MovementDirection.Backward;
        }
    }

    #endregion

    #region Switching Directions
    void SwitchDirection()
    {
        if (_currentMovementDirection == _positiveMovementDirection)
        {
            _currentMovementDirection = _negativeMovementDirection;
        }
        else
        {
            _currentMovementDirection = _positiveMovementDirection;
        }
    }

    void SwitchBasedOnDistance()
    {
        float distanceTravelled = Mathf.Abs(Vector3.Distance(_initalPosition, transform.position));
        if (distanceTravelled >= distanceTillSwitch)
        {
            SwitchDirection();
        }
    }

    Vector3 GetDirection(MovementDirection direction)
    {
        Vector3 _direction = new Vector3();
        switch (direction)
        {
            case MovementDirection.Up:
                _direction = Vector3.up;
                break;
            case MovementDirection.Down:
                _direction = Vector3.down;
                break;
            case MovementDirection.Left:
                _direction = Vector3.left;
                break;
            case MovementDirection.Right:
                _direction = Vector3.right;
                break;
            case MovementDirection.Forward:
                _direction = Vector3.forward;
                break;
            case MovementDirection.Backward:
                _direction = Vector3.back;
                break;
        }
        return _direction;
    }

    void SwitchBasedOnTime()
    {
        // Switch if timer runs out 
        if (_timeSinceLastSwitch >= timeTillSwitch)
        {
            SwitchDirection();
            _timeSinceLastSwitch = 0.0f;
        }
        // Otherwise update timer 
        else
        {
            _timeSinceLastSwitch += 1 * Time.deltaTime;
        }
    }

    #endregion

    #region Platform Movement
    void Update()
    {
        switch (control)
        {
            case Control.Distance:
                SwitchBasedOnDistance(); 
                break;
            case Control.Time:
                SwitchBasedOnTime();
                break;

        }

        Move(GetDirection(_currentMovementDirection), speed);
    }

    void Move(Vector3 direction, float speed)
    {
        // Calculate the movement vector 
        Vector3 movementVector = direction * speed;
        // Apply movement 
        transform.position += movementVector * Time.deltaTime;
    }
    #endregion
}