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

            // --- Logique Visuelle (HEAD) ---
            // On place l'objet sur la plaque et on le lie au grill
            go.transform.position = transform.position + (Vector3.up * 0.1f);
            go.transform.SetParent(this.transform);

            Ingredients ing = go.GetComponent<Ingredients>();
            if (ing != null) 
            {
                ingredientsTypes.Add(ing.IngredientType);
                ingredients.Add(ing); // Nécessaire pour FurnaceManager.cs
            }
        }

        // --- Logique de Recette (Branch closet-reparation) ---
        CheckForRecipes(); 
    }

    private void CheckForRecipes()
    {
        if (allRecipes == null || allRecipes.Count == 0) return;

        foreach (Recipe recipe in allRecipes)
        {
            if (IsRecipeComplete(recipe))
            {
                // On détruit les ingrédients utilisés
                foreach (var go in ingredientsContain)
                {
                    if (go != null) Destroy(go);
                }

                ingredientsContain.Clear();
                ingredientsTypes.Clear();
                ingredients.Clear();

                // On fait apparaître le résultat (ex: une assiette finie)
                if (recipe.resultPrefab != null)
                {
                    Instantiate(recipe.resultPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                }
                
                // On s'arrête après avoir trouvé une recette valide
                break; 
            }
        }
    }

    private bool IsRecipeComplete(Recipe recipe)
    {
        // On crée une copie temporaire pour vérifier les ingrédients un par un
        List<Ingredients.Type> tempIngredients = new List<Ingredients.Type>(ingredientsTypes);

        foreach (Ingredients.Type required in recipe.requiredIngredients)
        {
            if (tempIngredients.Contains(required))
            {
                tempIngredients.Remove(required);
            }
            else
            {
                return false;
            }
        }
        
        // Optionnel : vérifier si le nombre d'ingrédients correspond pile (pour éviter Steak + Steak = Salade)
        // return tempIngredients.Count == 0; 
        
        return true;
    }
}