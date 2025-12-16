using UnityEngine;

[CreateAssetMenu(fileName = "Nouvel Ingredient", menuName = "Cuisine/Ingredient")]
public class IngredientSO : ScriptableObject
{
    public string nom;
    public Texture image; // L'image de la carotte, du fromage, etc.
}