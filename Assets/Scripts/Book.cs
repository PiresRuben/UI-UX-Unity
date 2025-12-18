using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Book : MonoBehaviour
{
    [Header("Data")]
    public List<Recipe> allRecipes = new List<Recipe>();

    [Header("World Space UI Elements")]
    public RecipeSlot leftPageSlot;
    public RecipeSlot rightPageSlot;
    public Button prevBtn;
    public Button nextBtn;

    [Header("Screen Space Popups")]
    public GameObject readPanel;
    public GameObject addPanel;
    public CanvasGroup mainCanvasGroup;

    [Header("Read Popup Components")]
    public TextMeshProUGUI readTitle;
    public TextMeshProUGUI readDesc;
    public TextMeshProUGUI readIngredients;

    [Header("Add Popup Components")]
    public TMP_InputField inputTitle;
    public TMP_InputField inputDesc;
    public TMP_Dropdown ingredientDropdown;

    private int currentIndex = 0;

    void Start()
    {
        UpdatePages();
        CloseAllPopups();

        // Setup initial des boutons
        prevBtn.onClick.AddListener(PrevPage);
        nextBtn.onClick.AddListener(NextPage);
    }

    // --- GESTION DES PAGES ---

    void UpdatePages()
    {
        // Page Gauche
        if (currentIndex < allRecipes.Count)
            leftPageSlot.Setup(allRecipes[currentIndex]);
        else
            leftPageSlot.Setup(null);

        // Page Droite
        if (currentIndex + 1 < allRecipes.Count)
            rightPageSlot.Setup(allRecipes[currentIndex + 1]);
        else
            rightPageSlot.Setup(null);

        // Gestion état des boutons
        prevBtn.interactable = currentIndex > 0;
        nextBtn.interactable = currentIndex + 2 < allRecipes.Count;
    }

    public void NextPage()
    {
        if (currentIndex + 2 < allRecipes.Count)
        {
            currentIndex += 2;
            UpdatePages();
        }
    }

    public void PrevPage()
    {
        if (currentIndex - 2 >= 0)
        {
            currentIndex -= 2;
            UpdatePages();
        }
    }

    // --- GESTION POPUP LECTURE ---

    public void OpenReadModal(Recipe recipe)
    {
        readPanel.SetActive(true);
        readTitle.text = recipe.title;
        readDesc.text = recipe.description;

        // Convertir la liste d'ingrédients en un seul texte
        readIngredients.text = "Ingrédients:\n" + string.Join("\n- ", recipe.ingredients);

        ToggleGameInteraction(false);
    }

    // --- GESTION POPUP AJOUT ---

    public void OpenAddModal()
    {
        addPanel.SetActive(true);
        inputTitle.text = "";
        inputDesc.text = "";
        ToggleGameInteraction(false);
    }

    public void SaveNewRecipe()
    {
        Debug.Log("Tentative de sauvegarde...");

        if (string.IsNullOrEmpty(inputTitle.text))
        {
            Debug.LogError("Erreur : Le titre est vide !");
            return;
        }

        Recipe newRecipe = ScriptableObject.CreateInstance<Recipe>();
        newRecipe.title = inputTitle.text;
        newRecipe.description = inputDesc.text;
        newRecipe.ingredients = new List<string>();

        if (ingredientDropdown.options.Count > 0)
        {
            newRecipe.ingredients.Add(ingredientDropdown.options[ingredientDropdown.value].text);
        }
        else
        {
            Debug.LogWarning("Attention : Le Dropdown est vide, aucun ingrédient ajouté.");
        }

        allRecipes.Add(newRecipe);
        Debug.Log("Recette ajoutée à la liste ! Total : " + allRecipes.Count);

        UpdatePages();
        CloseAllPopups();
    }


    public void CloseAllPopups()
    {
        readPanel.SetActive(false);
        addPanel.SetActive(false);
        ToggleGameInteraction(true);
    }

    void ToggleGameInteraction(bool enable)
    {
        Cursor.visible = !enable;
        Cursor.lockState = enable ? CursorLockMode.Locked : CursorLockMode.None;
    }
}