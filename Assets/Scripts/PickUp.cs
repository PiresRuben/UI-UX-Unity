using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 3f;
    public LayerMask interactionLayers;

    [Header("Outline Style")]
    public Color highlightColor = Color.yellow;
    public float outlineWidth = 5f;

    [Header("State")]
    public bool _holding;
    public GameObject current;
    public GameObject last;

    [Header("Hands Configuration")]
    private Camera cam;
    public Camera leftHandCamera;
    public Camera rightHandCamera;
    public GameObject leftHand;
    public GameObject rightHand;

    [Header("UI")]
    public Canvas PickUpCanva;

    private Ingredients targetIngredient;

    private Vector3 leftHandOffset = Vector3.forward;
    private Vector3 rightHandOffset = Vector3.back;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Book.IsMenuOpen)
        {
            if (last != null) DisableOutline(last);
            last = null; 
            current = null; 
            PickUpCanva.enabled = false;
            return;
        }

        _holding = (leftHand != null || rightHand != null);

        MaintainHandPositions();

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, interactionLayers))
        {
            GameObject target = hit.collider.gameObject;
            if (target != last)
            {
                if (last != null) DisableOutline(last);
                current = target;
                EnableOutline(current);
                last = current;
            }

            targetIngredient = current.GetComponent<Ingredients>();
            if (targetIngredient != null)
            {
                PickUpCanva.enabled = true;
                PickUpCanva.transform.position = current.transform.position + Vector3.up * 0.5f;

                if (Input.GetMouseButtonDown(0) && leftHand == null)
                {
                    HandlePickUp(current, targetIngredient, true);
                    PickUpCanva.enabled = false;
                }
                else if (Input.GetMouseButtonDown(1) && rightHand == null)
                {
                    HandlePickUp(current, targetIngredient, false);
                    PickUpCanva.enabled = false;
                }
            }
        }
        else
        {
            if (last != null)
            {
                DisableOutline(last);
                last = null; 
                current = null; 
                PickUpCanva.enabled = false;
            }
        }
    }

    void MaintainHandPositions()
    {
        if (leftHand != null && leftHandCamera != null)
        {
            leftHand.transform.position = leftHandCamera.transform.position + leftHandCamera.transform.forward * leftHandOffset.z;
            leftHand.transform.rotation = leftHandCamera.transform.rotation;
        }

        if (rightHand != null && rightHandCamera != null)
        {
            rightHand.transform.position = rightHandCamera.transform.position + rightHandCamera.transform.forward * Mathf.Abs(rightHandOffset.z);
            rightHand.transform.rotation = rightHandCamera.transform.rotation;
        }
    }

    void HandlePickUp(GameObject obj, Ingredients ingScript, bool isLeft)
    {
        DisableOutline(obj);

        if (ingScript.currentContainer != null)
        {
            Container cont = ingScript.currentContainer;
            cont.ingredientsContain.Remove(obj);
            cont.RefreshTypes();
            ingScript.currentContainer = null;
        }

        PlaceInHand(obj, isLeft);
        current = null;
        last = null;
    }

    public void EquipFromMenu(GameObject prefab, bool isLeft)
    {
        if (prefab == null) return;
        GameObject newObj = Instantiate(prefab);

        if (newObj.GetComponent<Ingredients>() == null)
        {
            Debug.LogError("Le prefab dans le menu n'a pas le script Ingredients !");
        }

        if (isLeft && leftHand != null)
        {
            Destroy(leftHand);
            leftHand = null;
        }
        else if (!isLeft && rightHand != null)
        {
            Destroy(rightHand);
            rightHand = null;
        }

        PlaceInHand(newObj, isLeft);
    }

    void PlaceInHand(GameObject obj, bool isLeft)
    {
        if (isLeft)
        {
            leftHand = obj;
            leftHand.transform.SetParent(leftHandCamera.transform);
        }
        else
        {
            rightHand = obj;
            rightHand.transform.SetParent(rightHandCamera.transform);
        }

        obj.transform.localPosition = Vector3.forward;
        obj.transform.localRotation = Quaternion.identity;
      
        obj.layer = LayerMask.NameToLayer("Ingredients");

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
    }

    void EnableOutline(GameObject obj)
    {
        if (obj == null) return;
        if (!obj.TryGetComponent(out Outline outline))
        {
            outline = obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = highlightColor;
            outline.OutlineWidth = outlineWidth;
        }
        outline.enabled = true;
    }

    void DisableOutline(GameObject obj)
    {
        if (obj == null) return;
        if (obj.TryGetComponent(out Outline outline))
        {
            outline.enabled = false;
        }
    }
}