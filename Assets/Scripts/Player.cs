using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private InputAction interactAction;

    void Awake()
    {
        var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();

        interactAction = playerInput.actions.FindAction("Interact");
    }

    void OnEnable()
    {
        if (interactAction != null)
            interactAction.performed += OnInteract;
    }

    void OnDisable()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        RaycastHit hit;

        Vector3 origin = Camera.main != null ? Camera.main.transform.position : transform.position + Vector3.up * 1.5f;
        Vector3 direction = Camera.main != null ? Camera.main.transform.forward : transform.forward;

        Debug.DrawRay(origin, direction * 10f, Color.red, 2.0f);

        if (Physics.Raycast(origin, direction, out hit, 10f, LayerMask.GetMask("Interactable")))
        {
            Debug.Log("Object trouvé : " + hit.collider.name);
        }
    }
}