using UnityEngine;

[CreateAssetMenu(fileName = "Nouvel Ingredient", menuName = "Cuisine/Ingredient")]
public class IngredientSO : ScriptableObject
{
    public string nom;
    [TextArea] public string description;
    public Texture image;
    public GameObject prefab;
}