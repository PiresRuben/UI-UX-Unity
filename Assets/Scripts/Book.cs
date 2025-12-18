using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Book : MonoBehaviour
{
    public static bool IsMenuOpen = false;

    [Header("Données")]
    public List<Recipe> allRecipes = new List<Recipe>();
    private int currentIndex = 0;
    private List<string> tempIngredients = new List<string>();

    [Header("Pages UI")]
    public RecipeSlot leftPageSlot;
    public RecipeSlot rightPageSlot;
    public Button prevBtn;
    public Button nextBtn;

    [Header("Popups")]
    public GameObject readPanel;
    public GameObject addPanel;

    [Header("Lecture")]
    public TextMeshProUGUI readTitle;
    public TextMeshProUGUI readDesc;
    public TextMeshProUGUI readIngredients;

    [Header("Création")]
    public TMP_InputField inputTitle;
    public TMP_InputField inputDesc;
    public TMP_Dropdown ingredientDropdown;
    public TextMeshProUGUI tempIngredientsDisplay;

    void Start()
    {
        UpdatePages();
        CloseAllPopups();
    }

    public void AddIngredientFromDropdown()
    {
        if (ingredientDropdown.options.Count > 0)
        {
            string selected = ingredientDropdown.options[ingredientDropdown.value].text;
            if (!tempIngredients.Contains(selected))
            {
                tempIngredients.Add(selected);
                UpdateTempDisplay();
            }
        }
    }

    void UpdateTempDisplay()
    {
        if (tempIngredientsDisplay)
            tempIngredientsDisplay.text = "<b>Ingrédients ajoutés :</b>\n" + string.Join(", ", tempIngredients);
    }

    public void OpenReadModal(Recipe recipe)
    {
        IsMenuOpen = true;
        readPanel.SetActive(true);
        addPanel.SetActive(false);
        readTitle.text = "<b>" + recipe.title + "</b>";
        readDesc.text = recipe.description;
        readIngredients.text = "<b>Ingrédients :</b>\n- " + string.Join("\n- ", recipe.ingredients);
        ToggleInteraction(false);
    }

    public void OpenAddModal()
    {
        IsMenuOpen = true;
        readPanel.SetActive(false);
        addPanel.SetActive(true);
        inputTitle.text = "";
        inputDesc.text = "";
        tempIngredients.Clear();
        UpdateTempDisplay();
        ToggleInteraction(false);
    }

    public void SaveNewRecipe()
    {
        if (string.IsNullOrEmpty(inputTitle.text) || tempIngredients.Count == 0) return;

        Recipe newRecipe = ScriptableObject.CreateInstance<Recipe>();
        newRecipe.title = inputTitle.text;
        newRecipe.description = inputDesc.text;
        newRecipe.ingredients = new List<string>(tempIngredients);

        allRecipes.Add(newRecipe);
        UpdatePages();
        CloseAllPopups();
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

    public void UpdatePages()
    {
        if (leftPageSlot) leftPageSlot.Setup(currentIndex < allRecipes.Count ? allRecipes[currentIndex] : null);
        if (rightPageSlot) rightPageSlot.Setup(currentIndex + 1 < allRecipes.Count ? allRecipes[currentIndex + 1] : null);
        prevBtn.interactable = currentIndex > 0;
        nextBtn.interactable = currentIndex + 2 < allRecipes.Count;
    }

    public void NextPage() { if (currentIndex + 2 < allRecipes.Count) { currentIndex += 2; UpdatePages(); } }
    public void PrevPage() { if (currentIndex - 2 >= 0) { currentIndex -= 2; UpdatePages(); } }
}