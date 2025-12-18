using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectButton : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RawImage iconDisplay;

    private IngredientSO data;
    private CupboardMenuManager manager;

    // Pour le Drag & Drop
    private GameObject dragGhost;
    private Canvas canvas;

    public void Setup(IngredientSO ingredient, CupboardMenuManager menuManager)
    {
        data = ingredient;
        manager = menuManager;

        // Conversion Texture -> Texture (RawImage accepte Texture direct)
        iconDisplay.texture = data.image;

        // Récupérer le canvas racine pour le drag
        canvas = GetComponentInParent<Canvas>();
    }

    // 1. Clic simple : Afficher détails
    public void OnPointerDown(PointerEventData eventData)
    {
        manager.ShowItemDetails(data);
    }

    // 2. Début du Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Créer une image fantôme qui suit la souris
        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(canvas.transform, false);
        dragGhost.AddComponent<CanvasGroup>().blocksRaycasts = false; // Important pour que le Drop passe au travers

        RawImage img = dragGhost.AddComponent<RawImage>();
        img.texture = data.image;
        img.rectTransform.sizeDelta = new Vector2(50, 50); // Taille arbitraire
    }

    // 3. Pendant le Drag
    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            dragGhost.transform.position = eventData.position;
        }
    }

    // 4. Fin du Drag (Lâché)
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhost != null) Destroy(dragGhost);

        // Raycast UI pour voir si on a lâché sur une zone spécifique (Main UI par exemple)
        // Note : Pour faire simple, on check si on a lâché sur un objet qui a un handler de drop
        // Si tu as des slots UI "Main Gauche" / "Main Droite", ils doivent avoir IDropHandler.
    }

    // Getter pour le DropHandler des mains
    public IngredientSO GetData() { return data; }
}