using UnityEngine;
using UnityEngine.EventSystems;

public class HandSlotUI : MonoBehaviour, IDropHandler
{
    public bool isLeftHand; // Coche la case pour la main gauche, laisse vide pour la droite
    public CupboardMenuManager manager; // Glisse ton manager ici

    public void OnDrop(PointerEventData eventData)
    {
        // On récupère l'objet qui est en train d'être glissé
        ObjectButton draggedItem = eventData.pointerDrag.GetComponent<ObjectButton>();

        if (draggedItem != null)
        {
            // On appelle la fonction du manager selon la main
            if (isLeftHand) manager.PutInLeftHand();
            else manager.PutInRightHand();

            Debug.Log("Objet déposé dans la main " + (isLeftHand ? "Gauche" : "Droite"));
        }
    }
}