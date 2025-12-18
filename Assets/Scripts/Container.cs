using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [HideInInspector] public List<GameObject> ingredientsContain = new();
    public List<Ingredients.Type> ingredientsTypes = new();

    // Ajoutez la liste de toutes vos recettes possibles ici (à remplir dans l'inspecteur)
    public List<Recipe> allRecipes;

    public void RefreshTypes()
    {
        ingredientsTypes.Clear();
        foreach (var go in ingredientsContain)
        {
            if (go == null) continue;
            Ingredients ing = go.GetComponent<Ingredients>();
            if (ing != null) ingredientsTypes.Add(ing.IngredientType);
        }

        CheckForRecipes(); // On vérifie dès qu'on rafraîchit
    }

    private void CheckForRecipes()
    {
        foreach (Recipe recipe in allRecipes)
        {
            if (IsRecipeComplete(recipe))
            {
                Debug.Log("Recette trouvée : " + recipe.title);
                foreach (var go in ingredientsContain)
                {
                    Destroy(go);
                }
                ingredientsContain.Clear();
                ingredientsTypes.Clear();


                Instantiate(recipe.resultPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
    }

    private bool IsRecipeComplete(Recipe recipe)
    {
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
        return true;
    }
}