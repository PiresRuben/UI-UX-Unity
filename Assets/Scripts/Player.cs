using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public GameObject menuPrefab;
    public Transform leftCamAnchor;
    public Transform rightCamAnchor;

    private GameObject activeMenu;
    private Outline currentOutline;
    private GameObject currentContenor;
    private GameObject targetObject;
    private InputAction interactAction;

    void Awake()
    {
        UnityEngine.InputSystem.PlayerInput input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        interactAction = input.actions.FindAction("Player/Interact");
    }

    void OnEnable() => interactAction.performed += OnInteract;
    void OnDisable() => interactAction.performed -= OnInteract;

    void Update()
    {
        RaycastHit hit;
        Vector3 fwd = Camera.main.transform.forward;
        Vector3 pos = Camera.main.transform.position;

        if (Physics.Raycast(pos, fwd, out hit, 5f, LayerMask.GetMask("Interactable")))
        {
            currentContenor = null;
            SetHighlight(hit.collider.gameObject);
        }
        else if (Physics.Raycast(pos, fwd, out hit, 5f, LayerMask.GetMask("InteractableContenor")))
        {
            currentContenor = hit.collider.gameObject;
            SetHighlight(currentContenor);
        }

        else if (Physics.Raycast(pos, fwd, out hit, 5f, LayerMask.GetMask("Stove")))
        {
            if (hit.collider.TryGetComponent(out Stove stove))
            {
                SetHighlight(hit.collider.gameObject);
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    void SetHighlight(GameObject obj)
    {
        if (!obj.TryGetComponent(out Outline outline))
        {
            outline = obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5f;
        }

        if (currentOutline != outline)
        {
            if (currentOutline) currentOutline.enabled = false;
            outline.enabled = true;
            currentOutline = outline;
        }
    }

    void ClearHighlight()
    {
        if (currentOutline)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
        currentContenor = null;
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (activeMenu) Destroy(activeMenu);

        if (currentOutline != null && currentOutline.TryGetComponent(out Stove stove))
        {
            stove.ToggleInterface(transform);
            return;
        }

        if (currentOutline != null && currentContenor == null)
        {
            targetObject = currentOutline.gameObject;
            SpawnMenu(targetObject.transform.position, true);
        }
        else if (currentContenor != null)
        {
            SpawnMenu(currentContenor.transform.position, false);
        }
    }

    void SpawnMenu(Vector3 position, bool isPickingUp)
    {
        activeMenu = Instantiate(menuPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
        activeMenu.transform.LookAt(Camera.main.transform);
        activeMenu.transform.Rotate(0, 180, 0);

        var btnL = activeMenu.transform.Find("BtnLeft").GetComponent<Button>();
        var btnR = activeMenu.transform.Find("BtnRight").GetComponent<Button>();

        if (isPickingUp)
        {
            btnL.onClick.AddListener(() => SendToStudio(leftCamAnchor));
            btnR.onClick.AddListener(() => SendToStudio(rightCamAnchor));
        }
        else
        {
            btnL.onClick.AddListener(() => DropFromStudio(leftCamAnchor));
            btnR.onClick.AddListener(() => DropFromStudio(rightCamAnchor));
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void SendToStudio(Transform anchor)
    {
        if (targetObject == null) return;

        foreach (Transform child in anchor) Destroy(child.gameObject);

        targetObject.transform.SetParent(anchor);
        targetObject.transform.localPosition = Vector3.zero;
        targetObject.transform.localRotation = Quaternion.identity;

        if (targetObject.GetComponent<Rigidbody>()) targetObject.GetComponent<Rigidbody>().isKinematic = true;

        SetLayerRecursively(targetObject, LayerMask.NameToLayer("UI_3D"));

        CloseMenu();
    }

    void DropFromStudio(Transform anchor)
    {
        if (anchor.childCount == 0 || currentContenor == null) return;

        GameObject objToDrop = anchor.GetChild(0).gameObject;

        objToDrop.transform.SetParent(null);
        objToDrop.transform.position = currentContenor.transform.position + Vector3.up * 0.1f;
        objToDrop.transform.rotation = Quaternion.identity;

        if (objToDrop.GetComponent<Rigidbody>()) objToDrop.GetComponent<Rigidbody>().isKinematic = false;

        SetLayerRecursively(objToDrop, 0);

        CloseMenu();
    }

    void CloseMenu()
    {
        if (activeMenu) Destroy(activeMenu);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, layer);
    }
}