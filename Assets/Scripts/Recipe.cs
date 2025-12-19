using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public GameObject resultPrefab;
    public bool isDiscovered = false;

    [System.Serializable]
    public class IngredientRequirement
    {
        public Ingredients.Type type;
        public bool mustBeCooked;
    }

    public List<IngredientRequirement> requiredIngredients = new List<IngredientRequirement>();
}