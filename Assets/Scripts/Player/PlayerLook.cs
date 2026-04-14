using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerLook : MonoBehaviour
{
    // parameters
    public Transform cameraTransform;

    public float mouseSensitivity = 2f;

    [Range(-90f, 0f)]
    public float minPitch = -85f;

    [Range(0f, 90f)]
    public float maxPitch = 85f;
    
    [Range(0f, 0.2f)]
    public float smoothTime = 0f;

    // private variables
    float _pitch = 0f;
    float _yaw = 0f;

    Vector2 _smoothVelocity;
    Vector2 _currentMouseDelta;
    Vector2 _targetMouseDelta;

    // unity
    void Awake()
    {
        // Auto-assign if not set
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>()?.transform;

        // hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        _targetMouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y")
        );

        if (smoothTime > 0.001f)
        {
            _currentMouseDelta = Vector2.SmoothDamp(
                _currentMouseDelta,
                _targetMouseDelta,
                ref _smoothVelocity,
                smoothTime
            );
        }
        else
        {
            _currentMouseDelta = _targetMouseDelta;
        }

        _yaw += _currentMouseDelta.x * mouseSensitivity;
        _pitch -= _currentMouseDelta.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        // unlock cursor with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // lock cursor on left click if not already locked
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
