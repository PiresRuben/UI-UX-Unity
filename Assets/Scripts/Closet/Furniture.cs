using UnityEngine;
using System.Collections.Generic;

// Structure simple pour définir une étagère dans l'inspecteur
[System.Serializable]
public class ShelfData
{
    public string shelfName = "Etagère";
    public List<IngredientSO> ingredients;
}

[RequireComponent(typeof(Outline))] // Nécessite le script Outline (Asset Store ou custom)
public class Furniture : MonoBehaviour
{
    [Header("Contenu du placard")]
    public List<ShelfData> shelves; // Liste des étagères (onglets)

    [HideInInspector] public Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false; // Désactivé par défaut
    }
}