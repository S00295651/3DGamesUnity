using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerLook : MonoBehaviour
{
    private Vector2 lookInput;
    private float mouseX;
    private float mouseY;
    private float xRotation;

    [Header("Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] protected float HorizontalSensitivity = 10;
    [SerializeField] protected float VerticalSensitivity = 10;
    [SerializeField] private bool IsCursorVisible = true;

    private void Awake()
    {
        Cursor.visible = IsCursorVisible;
    }

    private void OnEnable()
    {
        InputManager.Actions.Game.Look.performed += OnLook;
    }

    private void OnDisable()
    {
        InputManager.Actions.Game.Look.performed -= OnLook;
    }

    public void OnLook(InputAction.CallbackContext obj)
    {
        lookInput = obj.ReadValue<Vector2>();

        UpdateLook();
    }

    protected virtual void UpdateLook()
    {
        mouseX = lookInput.x * HorizontalSensitivity * Time.deltaTime;
        mouseY = lookInput.y * VerticalSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
