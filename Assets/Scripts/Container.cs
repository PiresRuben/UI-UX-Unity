using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public List<GameObject> ingredientsContain = new();
    public List<Ingredients> ingredients = new();

    public void RefreshTypes()
    {
        ingredients.Clear();
        foreach (var go in ingredientsContain)
        {
            if (go == null) continue;

            go.transform.position = transform.position + (Vector3.up * 0.1f);
            go.transform.SetParent(this.transform);

            Ingredients ing = go.GetComponent<Ingredients>();
            if (ing != null)
            {
                ing.currentContainer = this;
                ingredients.Add(ing);
            }
        }
        CheckForRecipes();
    }

    private void CheckForRecipes()
    {
        foreach (Recipe recipe in Book.globalRecipes)
        {
            if (IsRecipeComplete(recipe))
            {
                foreach (var go in ingredientsContain) { if (go != null) Destroy(go); }
                ingredientsContain.Clear();
                ingredients.Clear();

                if (recipe.resultPrefab != null)
                    Instantiate(recipe.resultPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                break;
            }
        }
    }

    private bool IsRecipeComplete(Recipe recipe)
    {
        if (ingredients.Count != recipe.requiredIngredients.Count) return false;

        List<Ingredients> tempItems = new List<Ingredients>(ingredients);

        foreach (Recipe.IngredientRequirement req in recipe.requiredIngredients)
        {
            Ingredients match = tempItems.Find(i => i.IngredientType == req.type && i.isCooked == req.mustBeCooked);

            if (match != null) tempItems.Remove(match);
            else return false;
        }
        return tempItems.Count == 0;
    }
}