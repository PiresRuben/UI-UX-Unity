using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    [Header("UI Quête")]
    public GameObject containerQuete; // Le panneau complet (pour l'afficher/cacher)
    public TextMeshProUGUI titreRecette;
    public TextMeshProUGUI listeIngredients; // Ou 3 images si tu préfères

    // Cette fonction sera appelée par le Livre
    public void DemarrerQuete(RecetteSO recette)
    {
        containerQuete.SetActive(true);
        titreRecette.text = "Objectif : " + recette.nomRecette;

        // On construit la liste des ingrédients
        string texteIngredients = "";
        foreach (var ing in recette.ingredients)
        {
            texteIngredients += "- " + ing.nom + "\n";
        }
        listeIngredients.text = texteIngredients;

        Debug.Log("Quête lancée : " + recette.nomRecette);
    }
}