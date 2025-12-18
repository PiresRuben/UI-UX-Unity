using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ShelfButton : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI label;

    private int shelfIndex;
    private CupboardMenuManager manager;

    public void Setup(string name, int index, CupboardMenuManager menuManager)
    {
        label.text = name;
        shelfIndex = index;
        manager = menuManager;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        manager.LoadShelf(shelfIndex);
    }
}