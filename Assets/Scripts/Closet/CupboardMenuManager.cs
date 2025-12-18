using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CupboardMenuManager : MonoBehaviour
{
    [Header("Panneaux UI")]
    public GameObject menuPanel;
    public Transform tabsContainer;
    public Transform gridContainer;

    [Header("UI Détails de l'objet")]
    public GameObject detailsPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public Image itemPreviewImage;

    [Header("Prefabs")]
    public GameObject shelfButtonPrefab;
    public GameObject ingredientIconPrefab;

    [Header("Références Externes")]
    public PickUpController playerPickupController;

    [HideInInspector] public bool isOpen = false;
    private Furniture currentOpenFurniture;
    private IngredientSO currentSelectedIngredient;
    private int currentShelfIndex = 0; // NOUVEAU : Tracker le shelf actuel

    void Start()
    {
        CloseMenu();
    }

    // --- Gestion Ouverture / Fermeture ---

    public void OpenMenu(Furniture furniture)
    {
        currentOpenFurniture = furniture;
        isOpen = true;

        GameObject dragGhost = GameObject.Find("DragGhost");
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            Debug.Log("DragGhost trouvé et détruit à l'ouverture du menu.");
        }
        else
        {
            Debug.Log("DragGhost non trouvé");
        }

        menuPanel.SetActive(true);
        detailsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GenerateTabs();
    }

    public void CloseMenu()
    {
        isOpen = false;

        // NOUVEAU : Détruire le DragGhost avant de fermer le menu
        GameObject dragGhost = GameObject.Find("DragGhost");
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            Debug.Log("DragGhost détruit à la fermeture du menu.");
        }

        menuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- Génération de l'interface ---

    void GenerateTabs()
    {
        foreach (Transform child in tabsContainer) Destroy(child.gameObject);

        for (int i = 0; i < currentOpenFurniture.shelves.Count; i++)
        {
            GameObject btnObj = Instantiate(shelfButtonPrefab, tabsContainer);
            ShelfButton btnScript = btnObj.GetComponent<ShelfButton>();
            btnScript.Setup(currentOpenFurniture.shelves[i].shelfName, i, this);
        }

        if (currentOpenFurniture.shelves.Count > 0)
            LoadShelf(0);
    }

    public void LoadShelf(int index)
    {
        currentShelfIndex = index; // NOUVEAU : Sauvegarder l'index actuel

        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        List<IngredientSO> items = currentOpenFurniture.shelves[index].ingredients;

        foreach (IngredientSO item in items)
        {
            GameObject iconObj = Instantiate(ingredientIconPrefab, gridContainer);
            ObjectButton iconScript = iconObj.GetComponent<ObjectButton>();
            iconScript.Setup(item, this);
        }
    }

    // --- Gestion de la sélection ---

    public void ShowItemDetails(IngredientSO item)
    {
        currentSelectedIngredient = item;
        detailsPanel.SetActive(true);

        itemNameText.text = item.nom;
        itemDescText.text = item.description;

        if (item.image != null)
        {
            Texture2D tex = (Texture2D)item.image;
            itemPreviewImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
        
    // --- Actions des boutons "Prendre" ---

    public void PutInLeftHand()
    {
        if (currentSelectedIngredient != null && playerPickupController.leftHand == null)
            SpawnAndEquip(currentSelectedIngredient, true);
    }

    public void PutInRightHand()
    {
        if (currentSelectedIngredient != null && playerPickupController.rightHand == null)
            SpawnAndEquip(currentSelectedIngredient, false);
    }

    void SpawnAndEquip(IngredientSO item, bool isLeft)
    {
        if (playerPickupController == null)
        {
            Debug.LogError("Erreur : Le PickUpController n'est pas assigné dans le CupboardMenuManager !");
            return;
        }

        // NOUVEAU : Retirer l'objet de la liste du placard pour éviter la duplication
        if (currentOpenFurniture != null && currentShelfIndex < currentOpenFurniture.shelves.Count)
        {
            List<IngredientSO> currentShelfItems = currentOpenFurniture.shelves[currentShelfIndex].ingredients;
            if (currentShelfItems.Contains(item))
            {
                currentShelfItems.Remove(item);
                // Rafraîchir l'affichage de la grille
                LoadShelf(currentShelfIndex);
            }
        }

        playerPickupController.EquipFromMenu(item.prefab, isLeft);
        CloseMenu();
    }
}