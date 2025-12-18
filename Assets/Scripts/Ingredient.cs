using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Kitchen/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    public Sprite icon;
}