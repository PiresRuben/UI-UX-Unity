using UnityEngine;
using TMPro;
using UnityEngine.UI; // Nécessaire pour RawImage
using System.Collections.Generic;

public class BookManager : MonoBehaviour
{
    [Header("--- DONNÉES & LIENS ---")]
    public List<RecetteSO> listeRecettes;
    public QuestDisplay questDisplay;

    private int pageIndex = 0;

    [Header("--- PAGE GAUCHE ---")]
    public GameObject groupePageGauche;
    public TextMeshProUGUI titreG;
    public TextMeshProUGUI[] ingredientsTxtG;
    public RawImage[] ingredientsImgG; // <-- Changé de Image en RawImage
    public Button boutonChoisirG;

    [Header("--- PAGE DROITE (Recette) ---")]
    public GameObject groupePageDroite;
    public TextMeshProUGUI titreD;
    public TextMeshProUGUI[] ingredientsTxtD;
    public RawImage[] ingredientsImgD; // <-- Changé de Image en RawImage
    public Button boutonChoisirD;

    [Header("--- PAGE DROITE (Création) ---")]
    public GameObject groupeCreation;
    public Button boutonLancerCreation;

    void Start()
    {
        UpdateBookDisplay();
        if (boutonLancerCreation != null)
            boutonLancerCreation.onClick.AddListener(OuvrirInterfaceCreation);
    }

    public void PageSuivante()
    {
        if (pageIndex < listeRecettes.Count)
        {
            pageIndex += 2;
            UpdateBookDisplay();
        }
    }

    public void PagePrecedente()
    {
        if (pageIndex > 0)
        {
            pageIndex -= 2;
            UpdateBookDisplay();
        }
    }

    void UpdateBookDisplay()
    {
        if (pageIndex < listeRecettes.Count)
        {
            groupePageGauche.SetActive(true);
            RemplirPage(listeRecettes[pageIndex], titreG, ingredientsTxtG, ingredientsImgG);

            boutonChoisirG.onClick.RemoveAllListeners();
            int indexActuel = pageIndex;
            boutonChoisirG.onClick.AddListener(() => ChoisirRecette(listeRecettes[indexActuel]));
        }
        else
        {
            groupePageGauche.SetActive(false);
        }

        if (pageIndex + 1 < listeRecettes.Count)
        {
            groupePageDroite.SetActive(true);
            groupeCreation.SetActive(false);

            RemplirPage(listeRecettes[pageIndex + 1], titreD, ingredientsTxtD, ingredientsImgD);

            boutonChoisirD.onClick.RemoveAllListeners();
            int indexSuivant = pageIndex + 1;
            boutonChoisirD.onClick.AddListener(() => ChoisirRecette(listeRecettes[indexSuivant]));
        }
        else
        {
            groupePageDroite.SetActive(false);
            groupeCreation.SetActive(true);
        }
    }

    // Mise à jour de la fonction pour accepter RawImage[]
    void RemplirPage(RecetteSO data, TextMeshProUGUI titre, TextMeshProUGUI[] txtSlots, RawImage[] imgSlots)
    {
        titre.text = data.nomRecette;

        for (int i = 0; i < txtSlots.Length; i++)
        {
            if (i >= imgSlots.Length) break;

            if (i < data.ingredients.Count)
            {
                IngredientSO ing = data.ingredients[i];
                txtSlots[i].text = ing.nom;

                if (ing.image != null)
                {
                    imgSlots[i].texture = ing.image; // <-- On utilise .texture au lieu de .sprite
                    imgSlots[i].enabled = true;
                }
                else
                {
                    imgSlots[i].enabled = false;
                }
            }
            else
            {
                txtSlots[i].text = "";
                imgSlots[i].enabled = false;
            }
        }
    }

    void ChoisirRecette(RecetteSO recetteChoisie)
    {
        if (questDisplay != null)
            questDisplay.DemarrerQuete(recetteChoisie);
    }

    void OuvrirInterfaceCreation()
    {
        Debug.Log("Ouverture du menu de création...");
    }
}