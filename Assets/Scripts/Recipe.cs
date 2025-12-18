using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Kitchen/Recipe")]
public class Recipe : ScriptableObject
{
    public string title;
    [TextArea(5, 10)]
    public string description;
    public List<string> ingredients;
    public List<Ingredients.Type> requiredIngredients;

    public GameObject resultPrefab;
}