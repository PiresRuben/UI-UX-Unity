using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class Book : MonoBehaviour
{
    public static bool IsMenuOpen = false;

    // Cette liste contient TOUTES les recettes (celles du début + les nouvelles)
    public static List<Recipe> globalRecipes = new List<Recipe>();

    [Header("Données")]
    public List<Recipe> startingRecipes = new List<Recipe>();
    private int currentIndex = 0;

    private List<Ingredients.Type> tempIngredientsLogic = new List<Ingredients.Type>();
    private List<string> tempIngredientsDisplay = new List<string>();

    [Header("Réglages Création")]
    public GameObject defaultResultPrefab;

    [Header("UI Pages")]
    public RecipeSlot leftPageSlot;
    public RecipeSlot rightPageSlot;
    public Button prevBtn;
    public Button nextBtn;

    [Header("Popups")]
    public GameObject readPanel;
    public GameObject addPanel;

    [Header("Composants Lecture")]
    public TextMeshProUGUI readTitle;
    public TextMeshProUGUI readDesc;
    public TextMeshProUGUI readIngredients;

    [Header("Composants Création")]
    public TMP_InputField inputTitle;
    public TMP_InputField inputDesc;
    public TMP_Dropdown ingredientDropdown;
    public TextMeshProUGUI tempDisplay;

    void Start()
    {
        // Initialisation de la liste globale avec tes recettes de base (Salade, etc.)
        globalRecipes.Clear();
        foreach (Recipe r in startingRecipes)
        {
            globalRecipes.Add(r);
        }

        PopulateDropdown();
        UpdatePages(); // On affiche les premières pages
        CloseAllPopups();
    }

    // --- LOGIQUE D'AFFICHAGE (Ce qui manquait) ---
    public void UpdatePages()
    {
        // Page de Gauche
        if (currentIndex < globalRecipes.Count)
            leftPageSlot.Setup(globalRecipes[currentIndex]);
        else
            leftPageSlot.Setup(null);

        // Page de Droite
        if (currentIndex + 1 < globalRecipes.Count)
            rightPageSlot.Setup(globalRecipes[currentIndex + 1]);
        else
            rightPageSlot.Setup(null);

        // Activation des boutons de navigation
        prevBtn.interactable = currentIndex > 0;
        nextBtn.interactable = currentIndex + 2 < globalRecipes.Count;
    }

    public void NextPage() { if (currentIndex + 2 < globalRecipes.Count) { currentIndex += 2; UpdatePages(); } }
    public void PrevPage() { if (currentIndex - 2 >= 0) { currentIndex -= 2; UpdatePages(); } }

    // --- LOGIQUE DE CRÉATION ---
    void PopulateDropdown()
    {
        ingredientDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (string name in Enum.GetNames(typeof(Ingredients.Type))) { options.Add(name); }
        ingredientDropdown.AddOptions(options);
    }

    public void AddIngredientFromDropdown()
    {
        string selectedName = ingredientDropdown.options[ingredientDropdown.value].text;
        Ingredients.Type selectedType = (Ingredients.Type)Enum.Parse(typeof(Ingredients.Type), selectedName);

        tempIngredientsLogic.Add(selectedType);
        tempIngredientsDisplay.Add(selectedName);
        UpdateTempDisplay();
    }

    void UpdateTempDisplay()
    {
        if (tempDisplay) tempDisplay.text = "<b>Ingrédients :</b> " + string.Join(", ", tempIngredientsDisplay);
    }

    public void SaveNewRecipe()
    {
        if (string.IsNullOrEmpty(inputTitle.text) || tempIngredientsLogic.Count == 0) return;

        Recipe newRecipe = ScriptableObject.CreateInstance<Recipe>();
        newRecipe.title = inputTitle.text;
        newRecipe.description = inputDesc.text;
        newRecipe.ingredients = new List<string>(tempIngredientsDisplay);
        newRecipe.requiredIngredients = new List<Ingredients.Type>(tempIngredientsLogic);
        newRecipe.resultPrefab = defaultResultPrefab;

        globalRecipes.Add(newRecipe); // Ajout à la liste globale
        UpdatePages(); // Rafraîchit le livre pour voir la nouvelle recette
        CloseAllPopups();
    }

    public void OpenReadModal(Recipe recipe)
    {
        IsMenuOpen = true;
        readPanel.SetActive(true);
        addPanel.SetActive(false);

        readTitle.text = "<b>" + recipe.title + "</b>";
        readDesc.text = recipe.description;

        string listText = "<b>Ingrédients nécessaires :</b>\n";
        foreach (Ingredients.Type type in recipe.requiredIngredients)
        {
            listText += "- " + type.ToString() + "\n";
        }
        readIngredients.text = listText;

        ToggleInteraction(false);
    }

    public void OpenAddModal()
    {
        IsMenuOpen = true;
        addPanel.SetActive(true);
        readPanel.SetActive(false);
        inputTitle.text = ""; inputDesc.text = "";
        tempIngredientsLogic.Clear(); tempIngredientsDisplay.Clear();
        UpdateTempDisplay();
        ToggleInteraction(false);
    }

    public void CloseAllPopups() { IsMenuOpen = false; readPanel.SetActive(false); addPanel.SetActive(false); ToggleInteraction(true); }

    void ToggleInteraction(bool gamePlay)
    {
        Cursor.visible = !gamePlay;
        Cursor.lockState = gamePlay ? CursorLockMode.Locked : CursorLockMode.None;
    }
}