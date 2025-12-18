using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class Book : MonoBehaviour
{
    public static bool IsMenuOpen = false;

    // Liste partagée entre le livre et les grills
    public static List<Recipe> globalRecipes = new List<Recipe>();

    [Header("Données de Base")]
    public List<Recipe> startingRecipes = new List<Recipe>();
    private int currentIndex = 0;

    // Listes temporaires pour la création
    private List<Ingredients.Type> tempIngredientsLogic = new List<Ingredients.Type>();
    private List<string> tempIngredientsNames = new List<string>();

    [Header("Réglages de Création")]
    public GameObject defaultResultPrefab; // Objet qui spawn quand on réussit une recette créée

    [Header("UI Navigation")]
    public RecipeSlot leftPageSlot;
    public RecipeSlot rightPageSlot;
    public Button prevBtn;
    public Button nextBtn;

    [Header("Panels")]
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
    public TextMeshProUGUI creationIngredientsDisplay; // Le texte TMP qui montre la liste en temps réel

    void Start()
    {
        // On initialise la liste globale avec les recettes de départ
        globalRecipes.Clear();
        foreach (Recipe r in startingRecipes)
        {
            globalRecipes.Add(r);
        }

        PopulateDropdown();
        UpdatePages();
        CloseAllPopups();
    }

    // --- 1. LOGIQUE D'AFFICHAGE ET NAVIGATION ---
    public void UpdatePages()
    {
        if (leftPageSlot)
            leftPageSlot.Setup(currentIndex < globalRecipes.Count ? globalRecipes[currentIndex] : null);

        if (rightPageSlot)
            rightPageSlot.Setup(currentIndex + 1 < globalRecipes.Count ? globalRecipes[currentIndex + 1] : null);

        prevBtn.interactable = currentIndex > 0;
        nextBtn.interactable = currentIndex + 2 < globalRecipes.Count;
    }

    public void NextPage() { if (currentIndex + 2 < globalRecipes.Count) { currentIndex += 2; UpdatePages(); } }
    public void PrevPage() { if (currentIndex - 2 >= 0) { currentIndex -= 2; UpdatePages(); } }

    // --- 2. LOGIQUE DE CRÉATION DE RECETTE ---
    void PopulateDropdown()
    {
        ingredientDropdown.ClearOptions();
        List<string> options = new List<string>();
        // Récupère automatiquement tous les ingrédients du script Ingredients
        foreach (string name in Enum.GetNames(typeof(Ingredients.Type)))
        {
            options.Add(name);
        }
        ingredientDropdown.AddOptions(options);
    }

    public void AddIngredientFromDropdown()
    {
        string selectedName = ingredientDropdown.options[ingredientDropdown.value].text;

        // Conversion texte -> Enum technique
        Ingredients.Type selectedType = (Ingredients.Type)Enum.Parse(typeof(Ingredients.Type), selectedName);

        tempIngredientsLogic.Add(selectedType);
        tempIngredientsNames.Add(selectedName);

        UpdateCreationDisplay();
    }

    void UpdateCreationDisplay()
    {
        if (creationIngredientsDisplay != null)
        {
            string formattedList = string.Join(", ", tempIngredientsNames);
            creationIngredientsDisplay.text = "<b>Ingrédients ajoutés :</b>\n" + formattedList;
        }
    }

    public void SaveNewRecipe()
    {
        if (string.IsNullOrEmpty(inputTitle.text) || tempIngredientsLogic.Count == 0) return;

        // On crée une instance réelle de la recette
        Recipe newRecipe = ScriptableObject.CreateInstance<Recipe>();
        newRecipe.title = inputTitle.text;
        newRecipe.description = inputDesc.text;

        // On remplit la liste technique (Required Ingredients)
        newRecipe.requiredIngredients = new List<Ingredients.Type>(tempIngredientsLogic);
        newRecipe.resultPrefab = defaultResultPrefab;

        globalRecipes.Add(newRecipe);
        UpdatePages();
        CloseAllPopups();
    }

    // --- 3. MODALES ET SOURIS ---
    public void OpenReadModal(Recipe recipe)
    {
        if (recipe == null) return;
        IsMenuOpen = true;
        readPanel.SetActive(true);
        addPanel.SetActive(false);

        readTitle.text = "<b>" + recipe.title + "</b>";
        readDesc.text = recipe.description;

        // On affiche la liste technique (Required Ingredients)
        string listText = "<b>Ingrédients requis :</b>\n";
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

        // Reset des champs
        inputTitle.text = "";
        inputDesc.text = "";
        tempIngredientsLogic.Clear();
        tempIngredientsNames.Clear();
        UpdateCreationDisplay();

        ToggleInteraction(false);
    }

    public void CloseAllPopups()
    {
        IsMenuOpen = false;
        readPanel.SetActive(false);
        addPanel.SetActive(false);
        ToggleInteraction(true);
    }

    void ToggleInteraction(bool isPlaying)
    {
        if (isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Débloque la souris pour les panels
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}