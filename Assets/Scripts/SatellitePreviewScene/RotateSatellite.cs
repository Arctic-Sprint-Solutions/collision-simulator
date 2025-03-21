// https://www.youtube.com/watch?v=CiZvMc8aI3U

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This script allows the user to rotate a satellite model using mouse input.
/// </summary>
public class RotateSatellite : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset _action;

    /// <summary>
    /// Public property to get or set the input action asset.
    /// </summary>
    public InputActionAsset action
    {
        get => _action;
        set => _action = value;
    }

    // Input actions for mouse look and left click
    protected InputAction leftClickPressedInputAction { get; set; }
    protected InputAction mouseLookInputAction { get; set; }

    // Private variables to control rotation and camera reference
    private bool _rotateAllowed;
    private Camera _camera;

    [Header("Rotation Speed")]
    [SerializeField] private float _speed;

    [Header("Inverted Rotation")]
    [SerializeField] private bool _inverted;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes input actions.
    /// </summary>
    private void Awake()
    {
        InitalizeInputActions();
    }

    /// <summary>
    /// Called before the first frame update.
    /// Sets the reference to the main camera.
    /// </summary>
    private void Start()
    {
        _camera = Camera.main;
    }

    /// <summary>
    /// Called once per frame.
    /// Handles the rotation of the satellite based on mouse input.
    /// </summary>
    private void Update()
    {
        // Exit early if rotation is not allowed
        if (!_rotateAllowed)
            return;

        // Get the mouse movement delta
        Vector2 MouseDelta = GetMouseLookInput();

        // Scale the mouse delta by speed and deltaTime
        MouseDelta *= _speed * Time.deltaTime;

        // Apply rotation to the satellite object
        transform.Rotate(Vector3.up * (_inverted ? 1 : -1), MouseDelta.x, Space.World);
        transform.Rotate(Vector3.right * (_inverted ? 1 : -1), MouseDelta.y, Space.World);
    }

    /// <summary>
    /// Initializes the input actions for mouse look and left click.
    /// </summary>
    private void InitalizeInputActions()
    {
        // Find the "Left Click" action in the input action asset
        leftClickPressedInputAction = _action.FindAction("Left Click");
        if (leftClickPressedInputAction != null)
        {
            // Subscribe to the left click input action events
            leftClickPressedInputAction.started += OnLeftClickPressed;
            leftClickPressedInputAction.performed += OnLeftClickPressed;
            leftClickPressedInputAction.canceled += OnLeftClickPressed;
        }

        // Find the "Mouse Look" action in the input action asset
        mouseLookInputAction = _action.FindAction("Mouse Look");

        // Enable the input actions
        leftClickPressedInputAction?.Enable();
        mouseLookInputAction?.Enable();
    }

    /// <summary>
    /// Callback for the left mouse button input action.
    /// Enables or disables rotation based on the input context.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    protected virtual void OnLeftClickPressed(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            // Enable rotation when the left mouse button is pressed
            _rotateAllowed = true;
        }
        else if (context.canceled)
        {
            // Disable rotation when the left mouse button is released
            _rotateAllowed = false;
        }
    }

    /// <summary>
    /// Reads the mouse movement input from the input action.
    /// </summary>
    /// <returns>A Vector2 representing the mouse movement delta.</returns>
    protected virtual Vector2 GetMouseLookInput()
    {
        // Return the mouse movement delta if the input action is valid
        if (mouseLookInputAction != null)
            return mouseLookInputAction.ReadValue<Vector2>();

        // Return zero if the input action is not valid
        return Vector2.zero;
    }
}