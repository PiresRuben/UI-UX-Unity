using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class RecipeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Book bookManager;
    private Recipe myRecipe;
    private TextMeshProUGUI myText;
    private Color originalColor;
    public Color highlightColor = Color.yellow;

    void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        originalColor = myText.color;
    }
    public void Setup(Recipe recipe)
    {
        myRecipe = recipe;
        if (myRecipe != null)
        {
            myText.text = myRecipe.title;
        }
        else
        {
            myText.text = "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myRecipe != null) myText.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        myText.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (myRecipe != null)
        {
            bookManager.OpenReadModal(myRecipe);
        }
    }
}