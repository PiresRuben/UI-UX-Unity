using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public List<GameObject> ingredientsContain = new();
    public List<Ingredients.Type> ingredientsTypes = new();
    public List<Ingredients> ingredients = new();

    public void RefreshTypes()
    {
        ingredientsTypes.Clear();
        ingredients.Clear();

        foreach (var go in ingredientsContain)
        {
            if (go == null) continue;

            go.transform.position = transform.position + (Vector3.up * 0.1f);
            go.transform.SetParent(this.transform);

            Ingredients ing = go.GetComponent<Ingredients>();
            if (ing != null)
            {
                ingredientsTypes.Add(ing.IngredientType);
                ingredients.Add(ing);
            }
        }
    }
}