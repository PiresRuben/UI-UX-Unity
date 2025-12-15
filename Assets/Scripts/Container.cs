using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [HideInInspector] public List<GameObject> ingredientsContain = new();
    public List<Ingredients.Type> ingredientsTypes = new();
    [HideInInspector] public List<Ingredients> ingredients = new();

    public void RefreshTypes()
    {
        ingredientsTypes.Clear(); // très important !
        ingredients.Clear();

        foreach (var go in ingredientsContain)
        {
            if (go == null) continue;

            Ingredients ing = go.GetComponent<Ingredients>();
            if (ing != null)
            {
                ingredientsTypes.Add(ing.IngredientType);
                ingredients.Add(ing);
            }
        }
    }
}
