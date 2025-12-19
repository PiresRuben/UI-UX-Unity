using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour
{
    public Book bookManager;

    [Header("Composants Page")]
    public TextMeshProUGUI titleTextOnPage;
    public TextMeshProUGUI descriptionTextOnPage;
    public Button readMoreButton;

    public void Setup(Recipe recipe)
    {
        readMoreButton.onClick.RemoveAllListeners();

        if (recipe != null)
        {
            if (recipe.isDiscovered)
            {
                titleTextOnPage.text = "<b>" + recipe.title + "</b>";
                descriptionTextOnPage.text = recipe.description;
                readMoreButton.gameObject.SetActive(true);
                readMoreButton.onClick.AddListener(() => bookManager.OpenReadModal(recipe));
            }
            else
            {
                titleTextOnPage.text = "<b>????</b>";
                descriptionTextOnPage.text = "Combinaison inconnue. Essayez de créer cette recette pour la débloquer.";
                readMoreButton.gameObject.SetActive(false);
            }
        }
        else
        {
            titleTextOnPage.text = "<i>Page Vide</i>";
            descriptionTextOnPage.text = "";
            readMoreButton.gameObject.SetActive(false);
        }
    }
}