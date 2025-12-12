using UnityEngine;

public class Ingredients : MonoBehaviour
{
    public enum Type
    {
        Eggplant,
        Cheese,
        Carrots,
        Meat,
        Bread,
        Egg
    }

    [SerializeField] private Type ingredientType;
    public Type IngredientType => ingredientType;

    public Container currentContainer;
    public bool inContainer => currentContainer != null;


    private void Start()
    {

    }
}