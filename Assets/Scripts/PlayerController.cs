using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Paramètres de Mouvement")]
    [Tooltip("Vitesse de déplacement en mètres/seconde")]
    public float moveSpeed = 5.0f;
    [Tooltip("Force de la gravité")]
    public float gravity = -9.81f;
    [Tooltip("Hauteur du saut")]
    public float jumpHeight = 1.5f;

    [Header("Paramètres de Caméra")]
    [Tooltip("Sensibilité de la souris")]
    public float lookSensitivity = 1.0f;
    [Tooltip("Limite de rotation verticale (en degrés)")]
    public float lookXLimit = 85.0f;

    [Header("Références")]
    public Camera playerCamera;

    private CharacterController characterController;
    private Vector3 velocity;
    private float rotationX = 0;
    private PlayerInput inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        inputActions = new PlayerInput();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        characterController.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = lookInput.x * lookSensitivity * 0.1f;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = lookInput.y * lookSensitivity * 0.1f;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }
}