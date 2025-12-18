using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 3f;

    [Header("Debug")]
    public GameObject current;      // Objet actuellement visé
    public GameObject last;         // Dernier objet visé

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
        cam = Camera.main;
    }

    void Update()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * rayDistance, Color.red);
        RaycastHit hit;

        // Gestion de l'état _holding
        _holding = (leftHand != null || rightHand != null);

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

                // Clic Gauche (Ramasser ce qu'on vise)
                if (Input.GetMouseButtonDown(0) && leftHand == null)
                {
                    HandlePickUp(current, targetIngredient, true);
                    PickUpCanva.enabled = false;
                }

                // Clic Droit (Ramasser ce qu'on vise)
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

    // --- Nouvelle fonction pour gérer le ramassage standard (depuis le Raycast) ---
    void HandlePickUp(GameObject obj, Ingredients ingScript, bool isLeft)
    {
        DisableOutline(obj);

        // Retirer du container si nécessaire
        if (ingScript.currentContainer != null)
        {
            Container cont = ingScript.currentContainer;
            cont.ingredientsContain.Remove(obj);
            cont.RefreshTypes();
            ingScript.currentContainer = null;
        }

        // Placer dans la main
        PlaceInHand(obj, isLeft);

        // Reset variables de visée
        current = null;
        last = null;
    }

    // --- NOUVELLE FONCTION APPELÉE PAR LE MENU ---
    public void EquipFromMenu(GameObject prefab, bool isLeft)
    {
        // 1. On instancie le prefab qui vient du ScriptableObject
        GameObject newObj = Instantiate(prefab);

        // 2. On s'assure qu'il a bien le composant Ingredients (sécurité)
        if (newObj.GetComponent<Ingredients>() == null)
        {
            Debug.LogError("Le prefab dans le menu n'a pas le script Ingredients !");
        }

        // 3. Si la main est déjà pleine, on détruit l'ancien objet (ou on le lâche, selon ton choix)
        if (isLeft && leftHand != null) Destroy(leftHand);
        if (!isLeft && rightHand != null) Destroy(rightHand);

        // 4. On place l'objet
        PlaceInHand(newObj, isLeft);
    }

    // --- Logique commune de placement (TP devant la caméra lointaine) ---
    void PlaceInHand(GameObject obj, bool isLeft)
    {
        if (isLeft)
        {
            leftHand = obj;
            // Ta logique de position pour la main gauche
            leftHand.transform.position = leftHandCamera.transform.position + Vector3.forward;
            // Optionnel : aligner la rotation
            leftHand.transform.rotation = leftHandCamera.transform.rotation;
        }
        else
        {
            rightHand = obj;
            // Ta logique de position pour la main droite
            rightHand.transform.position = rightHandCamera.transform.position + Vector3.back;
            // Optionnel : aligner la rotation
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