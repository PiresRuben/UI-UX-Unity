using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class Book : MonoBehaviour
{
    public static bool IsMenuOpen = false;
    public static List<Recipe> globalRecipes = new List<Recipe>();

    [Header("Base de Données")]
    public List<Recipe> startingRecipes = new List<Recipe>();
    public GameObject failedResultPrefab;

    private int currentIndex = 0;
    private List<Recipe.IngredientRequirement> tempReqs = new();
    private List<string> tempNames = new();

    [Header("UI Navigation")]
    public RecipeSlot leftPageSlot;
    public RecipeSlot rightPageSlot;
    public Button prevBtn;
    public Button nextBtn;

    [Header("Panels")]
    public GameObject readPanel;
    public GameObject addPanel;

    [Header("Composants UI Lecture")]
    public TextMeshProUGUI readTitle;
    public TextMeshProUGUI readDesc;
    public TextMeshProUGUI readIngredients;

    [Header("Composants UI Création")]
    public TMP_InputField inputTitle;
    public TMP_InputField inputDesc;
    public TMP_Dropdown ingredientDropdown;
    public TextMeshProUGUI creationIngredientsDisplay;
    public Toggle cookedToggle;

    void Start()
    {
        globalRecipes.Clear();
        foreach (Recipe r in startingRecipes)
        {
            globalRecipes.Add(r);
        }

        PopulateDropdown();
        UpdatePages();
        CloseAllPopups();
    }

    void Update()
    {
        if (IsMenuOpen)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void UpdatePages()
    {
        if (leftPageSlot != null)
            leftPageSlot.Setup(currentIndex < globalRecipes.Count ? globalRecipes[currentIndex] : null);

        if (rightPageSlot != null)
            rightPageSlot.Setup(currentIndex + 1 < globalRecipes.Count ? globalRecipes[currentIndex + 1] : null);

        if (prevBtn) prevBtn.interactable = currentIndex > 0;
        if (nextBtn) nextBtn.interactable = currentIndex + 2 < globalRecipes.Count;
    }

    public void NextPage() { currentIndex += 2; UpdatePages(); }
    public void PrevPage() { currentIndex -= 2; UpdatePages(); }

    void PopulateDropdown()
    {
        if (ingredientDropdown == null) return;
        ingredientDropdown.ClearOptions();

        List<string> options = Enum.GetNames(typeof(Ingredients.Type)).ToList();
        ingredientDropdown.AddOptions(options);
    }

    public void AddIngredientFromDropdown()
    {
        string selectedName = ingredientDropdown.options[ingredientDropdown.value].text;
        Ingredients.Type selectedType = (Ingredients.Type)Enum.Parse(typeof(Ingredients.Type), selectedName);

        tempReqs.Add(new Recipe.IngredientRequirement { type = selectedType, mustBeCooked = cookedToggle.isOn });
        tempNames.Add(selectedName + (cookedToggle.isOn ? " (Cuit)" : " (Cru)"));

        UpdateCreationDisplay();
    }

    void UpdateCreationDisplay()
    {
        if (creationIngredientsDisplay)
            creationIngredientsDisplay.text = "<b>Ingrédients ajoutés :</b>\n" + string.Join(", ", tempNames);
    }

    public void SaveNewRecipe()
    {
        if (string.IsNullOrEmpty(inputTitle.text) || tempReqs.Count == 0) return;

        Recipe match = FindMatchingRecipe(tempReqs);

        Recipe newR = ScriptableObject.CreateInstance<Recipe>();
        newR.title = inputTitle.text;
        newR.description = inputDesc.text;
        newR.requiredIngredients = new List<Recipe.IngredientRequirement>(tempReqs);
        newR.isDiscovered = true;

        if (match != null)
        {
            newR.resultPrefab = match.resultPrefab;
            match.isDiscovered = true;
        }
        else
        {
            newR.resultPrefab = failedResultPrefab;
        }

        globalRecipes.Add(newR);
        UpdatePages();
        CloseAllPopups();
    }

    private Recipe FindMatchingRecipe(List<Recipe.IngredientRequirement> playerReqs)
    {
        foreach (Recipe r in startingRecipes)
        {
            if (AreIngredientsMatching(playerReqs, r.requiredIngredients)) return r;
        }
        return null;
    }

    private bool AreIngredientsMatching(List<Recipe.IngredientRequirement> list1, List<Recipe.IngredientRequirement> list2)
    {
        if (list1.Count != list2.Count) return false;

        List<Recipe.IngredientRequirement> checkList = new List<Recipe.IngredientRequirement>(list2);
        foreach (var pReq in list1)
        {
            var found = checkList.Find(c => c.type == pReq.type && c.mustBeCooked == pReq.mustBeCooked);
            if (found != null) checkList.Remove(found);
            else return false;
        }
        return checkList.Count == 0;
    }

    public void OpenReadModal(Recipe recipe)
    {
        if (recipe == null || !recipe.isDiscovered) return;

        IsMenuOpen = true;
        readPanel.SetActive(true);
        readTitle.text = "<b>" + recipe.title + "</b>";
        readDesc.text = recipe.description;

        string listText = "<b>Ingrédients requis :</b>\n";
        foreach (var req in recipe.requiredIngredients)
        {
            listText += "- " + req.type.ToString() + (req.mustBeCooked ? " (Cuit)" : " (Cru)") + "\n";
        }
        readIngredients.text = listText;

        ToggleInteraction(false);
    }

    public void OpenAddModal()
    {
        IsMenuOpen = true;
        addPanel.SetActive(true);
        readPanel.SetActive(false);

        inputTitle.text = "";
        inputDesc.text = "";
        tempReqs.Clear();
        tempNames.Clear();
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

    void ToggleInteraction(bool gamePlay)
    {
        Cursor.visible = !gamePlay;
        Cursor.lockState = gamePlay ? CursorLockMode.Locked : CursorLockMode.None;
    }
}