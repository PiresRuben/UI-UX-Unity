using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [HideInInspector] public List<GameObject> ingredientsContain = new();
    public List<Ingredients.Type> ingredientsTypes = new();

    public void RefreshTypes()
    {
        ingredientsTypes.Clear(); // très important !

        foreach (var go in ingredientsContain)
        {
            if (go == null) continue;

            Ingredients ing = go.GetComponent<Ingredients>();
            if (ing != null)
            {
                ingredientsTypes.Add(ing.IngredientType);
            }
        }
    }
}
