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
    public GameObject current;      // Objet actuellement vise
    public GameObject last;         // Dernier objet vise

    [Header("Hands Configuration")]
    private Camera cam;
    public Camera leftHandCamera;
    public Camera rightHandCamera;
    public GameObject leftHand;
    public GameObject rightHand;

    [Header("UI")]
    public Canvas PickUpCanva;

    private Ingredients targetIngredient;

    // Positions relatives pour maintenir les objets devant les cameras
    private Vector3 leftHandOffset = Vector3.forward;
    private Vector3 rightHandOffset = Vector3.back;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // 1. Securite : Si le menu du livre est ouvert, on arrete tout
        if (Book.IsMenuOpen)
        {
            if (last != null) DisableOutline(last);
            last = null; 
            current = null; 
            PickUpCanva.enabled = false;
            return;
        }

        // 2. Mise a jour de l'etat de portage
        _holding = (leftHand != null || rightHand != null);

        // 3. Maintenir les objets dans les mains devant leurs cameras respectives
        MaintainHandPositions();

        // 4. Logique de Raycast avec Interaction Layers
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

                // Clic Gauche : Main Gauche
                if (Input.GetMouseButtonDown(0) && leftHand == null)
                {
                    HandlePickUp(current, targetIngredient, true);
                    PickUpCanva.enabled = false;
                }
                // Clic Droit : Main Droite
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

    // Fonction pour forcer la position des objets devant les cameras de rendu des mains
    void MaintainHandPositions()
    {
        if (leftHand != null && leftHandCamera != null)
        {
            leftHand.transform.position = leftHandCamera.transform.position + leftHandCamera.transform.forward * leftHandOffset.z;
            leftHand.transform.rotation = leftHandCamera.transform.rotation;
        }

        if (rightHand != null && rightHandCamera != null)
        {
            // Utilisation de la valeur absolue pour l'offset z si necessaire selon l'orientation de la camera
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

        // Nettoyage de la main si elle est deja pleine
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
        // On definit le parent pour la hierarchie
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

        // On applique la position initiale (sera maintenue par MaintainHandPositions)
        obj.transform.localPosition = Vector3.forward;
        obj.transform.localRotation = Quaternion.identity;
        
        // Changement de layer pour eviter que l'objet ne bloque le Raycast de visee
        obj.layer = LayerMask.NameToLayer("Default");

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