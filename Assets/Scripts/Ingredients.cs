using UnityEngine;

public class Ingredients : MonoBehaviour
{
    public enum Type { Eggplant, Tomato, Steak, Chips, Salad, Cheese, BurgerBun }
    public Type IngredientType;

    [HideInInspector] public Container currentContainer;

    [Header("Cooking Settings")]
    public bool canBeCooked;
    public GameObject cookedPrefab;
    public bool isCooked = false;
    [HideInInspector] public float cookingTimer = 0f;
}