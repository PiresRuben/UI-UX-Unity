using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 3f;

    [Header("Debug")]
    public GameObject current;     // Objet actuellement visé
    public GameObject last;        // Dernier objet visé

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

    void Start()
    {
        cam = Camera.main; // Récupération caméra FPS
    }

    void Update()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * rayDistance, Color.red);

        RaycastHit hit;

        if (leftHand != null || rightHand != null)
        {
            _holding = true;
        }
        else
        {
            _holding = false;
        }

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance))
        {
            GameObject target = hit.collider.gameObject;
            if (target.layer == LayerMask.NameToLayer("Ingredients"))
            {
                // Désactiver l’outline de l'ancien si on change d’objet
                if (last != null && last != target)
                    DisableOutline(last);
                targetIngredient = target.GetComponent<Ingredients>();
                if (targetIngredient == null) { Debug.LogWarning("Y'a pas le script ingredients dessus"); return; }

                current = target;
                EnableOutline(current);
                last = current;
                PickUpCanva.enabled = true;
                PickUpCanva.transform.position = current.transform.position + Vector3.up;


                // Clic gauche = objet main gauche
                if (Input.GetMouseButtonDown(0) && leftHand == null)
                {
                    DisableOutline(current);
                    current.transform.position = leftHandCamera.transform.position + Vector3.forward;

                    if (targetIngredient.currentContainer != null)
                    {
                        Container cont = targetIngredient.currentContainer;

                        cont.ingredientsContain.Remove(current);
                        cont.RefreshTypes();

                        targetIngredient.currentContainer = null;
                    }

                    leftHand = current;
                    PickUpCanva.enabled = false;

                    current = null;
                    last = null;

                }

                // Clic droit = objet main droite
                if (Input.GetMouseButtonDown(1) && rightHand == null)
                {
                    DisableOutline(current);
                    current.transform.position = rightHandCamera.transform.position + Vector3.back;

                    if (targetIngredient.currentContainer != null)
                    {
                        Container cont = targetIngredient.currentContainer;

                        cont.ingredientsContain.Remove(current);
                        cont.RefreshTypes();

                        targetIngredient.currentContainer = null;
                    }

                    rightHand = current;
                    PickUpCanva.enabled = false;

                    current = null;
                    last = null;

                }
                return; // On évite d'appeler ClearOutline()
            }
        }

        // Si on ne vise rien ou non-interactable, on nettoie proprement
        ClearOutline();
        PickUpCanva.enabled = false;
    }

    void EnableOutline(GameObject obj)
    {
        if (obj == null) return;

        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = true;
    }

    void DisableOutline(GameObject obj)
    {
        if (obj == null) return;

        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }

    void ClearOutline()
    {
        if (last != null)
        {
            DisableOutline(last);
            last = null;
        }

        current = null;
    }
}
