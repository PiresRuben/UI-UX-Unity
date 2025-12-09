using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Rigidbody rigidBody = null;

    [SerializeField] private Transform root = null;
    [SerializeField] private Transform head = null;

    private Vector3 input = Vector3.zero;
    private Vector2 rotationInput;
    private Vector2 currentRotation;

    [SerializeField] private Vector2 minMaxYaw = new Vector2(-89f, 90f);

    private void Reset()
    {
        rigidBody = GetComponent<Rigidbody>();
        //interactionMask = LayerMask.GetMask("Interactable");
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Player_OnMove(CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        input.z = input.y;
        input.y = 0;
    }


    public void Player_OnLook(CallbackContext context)
    {
        rotationInput = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        currentRotation.x += -rotationInput.y * rotationSpeed * Time.deltaTime;
        currentRotation.y += rotationInput.x * rotationSpeed * Time.deltaTime;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minMaxYaw.x, minMaxYaw.y);

        root.localRotation = Quaternion.Euler(0, currentRotation.y, 0);
        head.localRotation = Quaternion.Euler(currentRotation.x, 0, 0);
    }

    private void FixedUpdate()
    {
        rigidBody.linearVelocity = root.rotation * (speed * input.normalized);
    }
}