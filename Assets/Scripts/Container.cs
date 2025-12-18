using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [Header("Contenu du Container")]
    public List<GameObject> ingredientsContain = new();
    public List<Ingredients.Type> ingredientsTypes = new();
    public List<Ingredients> ingredients = new(); // Gardé pour la compatibilité avec FurnaceManager

    [Header("Système de Recettes")]
    // Ajoutez vos ScriptableObjects de recettes ici dans l'inspecteur
    public List<Recipe> allRecipes = new List<Recipe>();

    public void RefreshTypes()
    {
        ingredientsTypes.Clear();
        ingredients.Clear();

        foreach (var go in ingredientsContain)
        {
            if (go == null) continue;

            // On positionne l'objet sur le grill
            go.transform.position = transform.position + (Vector3.up * 0.1f);
            go.transform.SetParent(this.transform);

            Ingredients ing = go.GetComponent<Ingredients>();
            if (ing != null)
            {
                // --- CETTE LIGNE EST LA CLÉ ---
                ing.currentContainer = this;

                ingredientsTypes.Add(ing.IngredientType);
                ingredients.Add(ing);
            }
        }

        CheckForRecipes();
    }

    private void CheckForRecipes()
    {
        // On va chercher les recettes directement dans le livre (globales)
        foreach (Recipe recipe in Book.globalRecipes)
        {
            if (IsRecipeComplete(recipe))
            {
                // Debug pour confirmer la réussite
                Debug.Log("<color=green>Recette créée détectée : </color>" + recipe.title);

                foreach (var go in ingredientsContain)
                {
                    if (go != null) Destroy(go);
                }

                ingredientsContain.Clear();
                ingredientsTypes.Clear();
                ingredients.Clear();

                // Création du résultat au centre du grill
                if (recipe.resultPrefab != null)
                {
                    Instantiate(recipe.resultPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                }
                break;
            }
        }
    }

    private bool IsRecipeComplete(Recipe recipe)
    {
        // On travaille bien sur la liste technique (Required Ingredients)
        List<Ingredients.Type> tempIngredients = new List<Ingredients.Type>(ingredientsTypes);

        foreach (Ingredients.Type required in recipe.requiredIngredients)
        {
            if (tempIngredients.Contains(required))
            {
                tempIngredients.Remove(required);
            }
            else
            {
                return false; // Il manque un ingrédient de la liste "Required"
            }
        }

        // Si on veut une recette exacte (pas d'ingrédients en trop)
        return tempIngredients.Count == 0;
    }
}