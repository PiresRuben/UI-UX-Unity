using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectButton : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RawImage iconDisplay;
    private IngredientSO data;
    private CupboardMenuManager manager;
    private GameObject dragGhost;
    private Canvas canvas;

    public void Setup(IngredientSO ingredient, CupboardMenuManager menuManager)
    {
        data = ingredient;
        manager = menuManager;
        iconDisplay.texture = data.image;
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        manager.ShowItemDetails(data);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(canvas.transform, false);

        CanvasGroup cg = dragGhost.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 0.6f; // NOUVEAU : Rendre le ghost semi-transparent

        RawImage img = dragGhost.AddComponent<RawImage>();
        img.texture = data.image;
        img.rectTransform.sizeDelta = new Vector2(50, 50);

        dragGhost.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            dragGhost.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // MODIFIÉ : Destruction immédiate et forcée
        if (dragGhost != null)
        {
            Debug.LogWarning("dragGhost détruit");
            Destroy(dragGhost);
            dragGhost = null; // Important : remettre à null
        }
        else
        {
             Debug.LogWarning("dragGhost était déjà null lors de OnEndDrag.");
        }
    }

    public IngredientSO GetData() { return data; }
}