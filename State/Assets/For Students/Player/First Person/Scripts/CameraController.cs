using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Settings")]
    public float mouseSensitivity = 100f;
    [Tooltip("How far upwards in degrees the player can look.")]
    public float upperLookLimit;
    [Tooltip("How far downwards in degrees the player can look.")]
    public float lowerLookLimit;

    private float xRotation = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, lowerLookLimit, upperLookLimit);

        transform.parent.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
    }
}