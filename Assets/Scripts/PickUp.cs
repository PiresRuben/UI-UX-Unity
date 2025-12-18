using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 3f;

    [Header("Debug")]
    public GameObject current;
    public GameObject last;
    public bool _holding;
    private Ingredients targetIngredient;

    [Header("HandsCamera")]
    private Camera cam;
    public Camera leftHandCamera;
    public Camera rightHandCamera;

    public GameObject leftHand;
    public GameObject rightHand;

    [Header("CanvaUI")]
    public Canvas PickUpCanva;

    // NOUVEAU : Positions relatives pour maintenir les objets devant les caméras
    private Vector3 leftHandOffset = Vector3.forward;
    private Vector3 rightHandOffset = Vector3.back;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * rayDistance, Color.red);
        RaycastHit hit;

        _holding = (leftHand != null || rightHand != null);

        // NOUVEAU : Maintenir les objets dans les mains devant leurs caméras
        MaintainHandPositions();

        // --- Logique de Raycast existante ---
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance))
        {
            GameObject target = hit.collider.gameObject;
            if (target.layer == LayerMask.NameToLayer("Ingredients"))
            {
                if (last != null && last != target) DisableOutline(last);

                targetIngredient = target.GetComponent<Ingredients>();
                if (targetIngredient == null) return;

                current = target;
                EnableOutline(current);
                last = current;

                PickUpCanva.enabled = true;
                PickUpCanva.transform.position = current.transform.position + Vector3.up;

                if (Input.GetMouseButtonDown(0) && leftHand == null)
                {
                    HandlePickUp(current, targetIngredient, true);
                    PickUpCanva.enabled = false;
                }

                if (Input.GetMouseButtonDown(1) && rightHand == null)
                {
                    HandlePickUp(current, targetIngredient, false);
                    PickUpCanva.enabled = false;
                }
                return;
            }
        }

        ClearOutline();
        PickUpCanva.enabled = false;
    }

    // NOUVEAU : Fonction pour maintenir les objets devant leurs caméras
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
        GameObject newObj = Instantiate(prefab);

        if (newObj.GetComponent<Ingredients>() == null)
        {
            Debug.LogError("Le prefab dans le menu n'a pas le script Ingredients !");
        }

        // MODIFIÉ : Détruire proprement l'ancien objet si la main est pleine
        if (isLeft && leftHand != null)
        {
            Destroy(leftHand);
            leftHand = null;
        }
        if (!isLeft && rightHand != null)
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
            leftHand.transform.position = leftHandCamera.transform.position + leftHandCamera.transform.forward * leftHandOffset.z;
            leftHand.transform.rotation = leftHandCamera.transform.rotation;
        }
        else
        {
            rightHand = obj;
            rightHand.transform.position = rightHandCamera.transform.position + rightHandCamera.transform.forward * Mathf.Abs(rightHandOffset.z);
            rightHand.transform.rotation = rightHandCamera.transform.rotation;
        }
    }

    // --- Utilitaires Outline ---
    void EnableOutline(GameObject obj)
    {
        if (obj == null) return;
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null) outline.enabled = true;
    }

    void DisableOutline(GameObject obj)
    {
        if (obj == null) return;
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
    }

    void ClearOutline()
    {
        if (last != null) { DisableOutline(last); last = null; }
        current = null;
    }
}