using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nécessaire pour TextMeshPro
using System.Collections.Generic;

public class CupboardMenuManager : MonoBehaviour
{
    [Header("Panneaux UI")]
    public GameObject menuPanel;        // Le panel global (fond noir/flou)
    public Transform tabsContainer;     // Le parent (Horizontal Layout) pour les onglets
    public Transform gridContainer;     // Le parent (Grid Layout) pour les icônes

    [Header("UI Détails de l'objet")]
    public GameObject detailsPanel;     // Le panel de droite qui affiche les infos
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public Image itemPreviewImage;      // L'image UI pour la prévisualisation

    [Header("Prefabs")]
    public GameObject shelfButtonPrefab;    // Prefab du bouton onglet (avec script ShelfButton)
    public GameObject ingredientIconPrefab; // Prefab de l'item grille (avec script ObjectButton)

    [Header("Références Externes")]
    public PickUpController playerPickupController; // Référence OBLIGATOIRE vers ton joueur

    // Variables d'état interne
    [HideInInspector] public bool isOpen = false;
    private Furniture currentOpenFurniture;
    private IngredientSO currentSelectedIngredient;

    void Start()
    {
        CloseMenu(); // On s'assure que le menu est fermé au lancement
    }

    // --- Gestion Ouverture / Fermeture ---

    public void OpenMenu(Furniture furniture)
    {
        currentOpenFurniture = furniture;
        isOpen = true;

        // Afficher l'interface
        menuPanel.SetActive(true);
        detailsPanel.SetActive(false); // On cache les détails tant qu'on n'a rien cliqué

        // Gestion de la souris (Mode Menu)
        Cursor.lockState = CursorLockMode.None; // Déverrouille la souris
        Cursor.visible = true;                  // Rend la souris visible

        GenerateTabs();
    }

    public void CloseMenu()
    {
        isOpen = false;
        menuPanel.SetActive(false);

        // Gestion de la souris (Mode FPS)
        Cursor.lockState = CursorLockMode.Locked; // Verrouille la souris au centre
        Cursor.visible = false;                   // Cache la souris
    }

    // --- Génération de l'interface ---

    void GenerateTabs()
    {
        // 1. Nettoyage des anciens onglets
        foreach (Transform child in tabsContainer) Destroy(child.gameObject);

        // 2. Création des nouveaux boutons d'onglets
        for (int i = 0; i < currentOpenFurniture.shelves.Count; i++)
        {
            GameObject btnObj = Instantiate(shelfButtonPrefab, tabsContainer);
            ShelfButton btnScript = btnObj.GetComponent<ShelfButton>();

            // On configure le bouton (Nom de l'étagère, son index, et ce manager)
            btnScript.Setup(currentOpenFurniture.shelves[i].shelfName, i, this);
        }

        // 3. Charger la première étagère par défaut s'il y en a une
        if (currentOpenFurniture.shelves.Count > 0)
            LoadShelf(0);
    }

    // Appelé par le ShelfButton quand on clique dessus
    public void LoadShelf(int index)
    {
        // 1. Nettoyage de la grille précédente
        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        // 2. Récupération de la liste d'ingrédients pour cette étagère
        List<IngredientSO> items = currentOpenFurniture.shelves[index].ingredients;

        // 3. Création des icônes dans la grille
        foreach (IngredientSO item in items)
        {
            GameObject iconObj = Instantiate(ingredientIconPrefab, gridContainer);
            ObjectButton iconScript = iconObj.GetComponent<ObjectButton>();

            // On configure l'icône
            iconScript.Setup(item, this);
        }
    }

    // --- Gestion de la sélection ---

    // Appelé par le ObjectButton quand on clique dessus
    public void ShowItemDetails(IngredientSO item)
    {
        currentSelectedIngredient = item;
        detailsPanel.SetActive(true);

        itemNameText.text = item.nom;
        itemDescText.text = item.description;

        // Conversion Texture (ScriptableObject) vers Sprite (UI Image)
        if (item.image != null)
        {
            Texture2D tex = (Texture2D)item.image;
            itemPreviewImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    // --- Actions des boutons "Prendre" ---

    // Lier ce bouton à l'événement OnClick dans l'inspecteur Unity
    public void PutInLeftHand()
    {
        if (currentSelectedIngredient != null)
            SpawnAndEquip(currentSelectedIngredient, true);
    }

    // Lier ce bouton à l'événement OnClick dans l'inspecteur Unity
    public void PutInRightHand()
    {
        if (currentSelectedIngredient != null)
            SpawnAndEquip(currentSelectedIngredient, false);
    }

    void SpawnAndEquip(IngredientSO item, bool isLeft)
    {
        if (playerPickupController == null)
        {
            Debug.LogError("Erreur : Le PickUpController n'est pas assigné dans le CupboardMenuManager !");
            return;
        }

        // C'est ici qu'on appelle la nouvelle fonction de ton PickUpController
        // Elle va gérer l'instanciation et la téléportation devant tes caméras lointaines
        playerPickupController.EquipFromMenu(item.prefab, isLeft);

        // On ferme le menu une fois l'objet pris
        CloseMenu();
    }
}