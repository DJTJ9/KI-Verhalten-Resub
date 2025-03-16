using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    public RigidbodyMovement RigidbodyMovement;
    public CameraRotator CameraRotator;

    [Header("Interact")]
    public Transform MainCameraTransform;
    public Transform ObjectGrabPoint;

    [Header("Input")]
    public PlayerInput PlayerInput;

    [Header("Settings")]
    public float LookSensitivity = 2;

    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private InputAction lookInputAction;
    private InputAction grabInputAction;
    private InputAction dropInputAction;

    private GrabbableObject grabbableObject;
    private float pickUpDistance = 2f;

    private void Awake() {
        MapInputActions();
    }


    /// <summary>
    /// Sets cursor lock mode on left click to locked and on escape to none.
    /// Gets move direction from input and moves rigidbody into this direction.
    /// Rotates the rigidbody horizontally if cursor lock mode is locked.
    /// </summary>
    private void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) Cursor.lockState = CursorLockMode.Locked;
        if (Keyboard.current.escapeKey.wasPressedThisFrame) Cursor.lockState = CursorLockMode.None;


        var moveDirection = GetMoveDirectionFromInput();
        RigidbodyMovement.Move(moveDirection);

        if (Cursor.lockState == CursorLockMode.Locked) {
            var rotation = GetRotationFromInput();
            RigidbodyMovement.RotateHorizontal(rotation.x * LookSensitivity);
        }

    }

    /// <summary>
    /// Rotates camera vertically if cursor lock mode is locked.
    /// </summary>
    private void LateUpdate() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            if (CameraRotator != null)
                UpdateCamera();
        }
    }

    /// <summary>
    /// Gets rotation from input
    /// Rotates camera in the direction of the rotation input
    /// </summary>
    private void UpdateCamera() {
        var rotation = GetRotationFromInput();
        CameraRotator.Rotate(rotation.y);
    }

    /// <summary>
    /// Maps the input actions
    /// Subcribes the OnJumpInput method to the jump started action
    /// </summary>
    private void MapInputActions() {
        moveInputAction = PlayerInput.actions["Move"];

        jumpInputAction = PlayerInput.actions["Jump"];
        jumpInputAction.started += OnJumpInput;

        lookInputAction = PlayerInput.actions["Look"];

        grabInputAction = PlayerInput.actions["Grab"];
        grabInputAction.started += OnGrabInput;

        dropInputAction = PlayerInput.actions["Drop"];
        dropInputAction.started += OnDropInput;
    }

    private void OnJumpInput(InputAction.CallbackContext _context) {
        if (_context.phase == InputActionPhase.Started)
            RigidbodyMovement.Jump();
    }

    /// <summary>
    /// Gets the horizontal move direction from the input
    /// Converts this input into a 3D vector and returns it
    /// </summary>
    private Vector3 GetMoveDirectionFromInput() {
        var moveInput = moveInputAction.ReadValue<Vector2>();
        return new Vector3(moveInput.x, 0f, moveInput.y);

    }

    /// <summary>
    /// Gets the rotation input and returns it
    /// </summary>
    private Vector2 GetRotationFromInput() {
        return lookInputAction.ReadValue<Vector2>();
    }

    private void OnGrabInput(InputAction.CallbackContext _context) {
        if (_context.phase == InputActionPhase.Started)
            if (grabbableObject == null) {
                if (Physics.Raycast(MainCameraTransform.position, MainCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance)) {
                    if (raycastHit.transform.TryGetComponent(out GrabbableObject grabbableObject)) {
                        grabbableObject.Grab(ObjectGrabPoint);
                        this.grabbableObject = grabbableObject;
                    }
                }
            }
    }

    private void OnDropInput(InputAction.CallbackContext _context) {
        grabbableObject.Drop();
        grabbableObject = null;
    }
}
