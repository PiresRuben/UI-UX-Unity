using UnityEngine;

public class Ingredients : MonoBehaviour
{
    public enum Type { Eggplant, Tomato, Steak, Chips, Salad, Cheese }
    public Type IngredientType;

    // --- CETTE VARIABLE RÉPARE TES ERREURS ---
    [HideInInspector] public Container currentContainer;

    [Header("Cooking Settings")]
    public bool canBeCooked;       // Cocher seulement pour les steaks, etc.
    public GameObject cookedPrefab; // Le prefab de l'objet une fois cuit
    public bool isCooked = false;
    [HideInInspector] public float cookingTimer = 0f;
}