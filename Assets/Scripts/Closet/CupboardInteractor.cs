using UnityEngine;

public class CupboardInteractor : MonoBehaviour
{
    [Header("Camera")]
    private Camera cam;

    [Header("Paramètres")]
    public float rayDistance = 3f;
    public LayerMask interactableLayer; // Assure-toi de mettre tes placards sur ce Layer

    [Header("UI Reference")]
    public CupboardMenuManager menuManager; // Référence vers le script UI qu'on va créer

    private Furniture currentFurniture;
    private Furniture lastFurniture;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Si le menu est ouvert, on bloque le raycast pour éviter les conflits
        if (menuManager.isOpen) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, interactableLayer))
        {
            Furniture target = hit.collider.GetComponent<Furniture>();

            if (target != null)
            {
                HandleOutline(target);

                // Ouverture du menu au clic gauche
                if (Input.GetMouseButtonDown(0))
                {
                    menuManager.OpenMenu(target);
                    // Optionnel : Désactiver l'outline quand le menu s'ouvre
                    DisableOutline(target);
                }
                return;
            }
        }

        ClearOutline();
    }

    // --- Gestion de l'Outline (ta logique adaptée) ---

    void HandleOutline(Furniture target)
    {
        if (lastFurniture != null && lastFurniture != target)
        {
            DisableOutline(lastFurniture);
        }

        currentFurniture = target;
        EnableOutline(currentFurniture);
        lastFurniture = currentFurniture;
    }

    void EnableOutline(Furniture furn)
    {
        if (furn != null && furn.outline != null) furn.outline.enabled = true;
    }

    void DisableOutline(Furniture furn)
    {
        if (furn != null && furn.outline != null) furn.outline.enabled = false;
    }

    void ClearOutline()
    {
        if (lastFurniture != null)
        {
            DisableOutline(lastFurniture);
            lastFurniture = null;
        }
        currentFurniture = null;
    }
}