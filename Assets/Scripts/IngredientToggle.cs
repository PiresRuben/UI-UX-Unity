using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientToggle : MonoBehaviour
{
    public Ingredient myIngredientData;
    public Image iconDisplay;
    public TextMeshProUGUI nameText;
    public Toggle myToggle;

    public void Setup(Ingredient data)
    {
        myIngredientData = data;
        if (data.icon != null) iconDisplay.sprite = data.icon;
        nameText.text = data.ingredientName;
        myToggle.isOn = false;
    }
}