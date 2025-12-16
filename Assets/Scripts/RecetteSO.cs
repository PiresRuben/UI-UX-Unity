using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Nouvelle Recette", menuName = "Cuisine/Recette")]
public class RecetteSO : ScriptableObject
{
    public string nomRecette;
    public List<IngredientSO> ingredients = new List<IngredientSO>(); // Liste de 3 ingrédients
    // Tu pourras ajouter une description ou une difficulté ici plus tard
}