using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandSlotUI : MonoBehaviour, IDropHandler
{
    [Header("Configuration")]
    public bool isLeftHand;
    public CupboardMenuManager manager;

    [Header("RenderTexture Display")]
    public RawImage handDisplay; // NOUVEAU : L'image UI qui affiche la RenderTexture
    public Camera handCamera;    // NOUVEAU : La caméra qui rend la main (leftHandCamera ou rightHandCamera)

    void Start()
    {
        // NOUVEAU : Configuration automatique de la RenderTexture
        if (handCamera != null && handDisplay != null)
        {
            // Créer une RenderTexture si elle n'existe pas déjà
            if (handCamera.targetTexture == null)
            {
                RenderTexture rt = new RenderTexture(256, 256, 16);
                handCamera.targetTexture = rt;
            }

            // Assigner la RenderTexture à l'image UI
            handDisplay.texture = handCamera.targetTexture;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ObjectButton draggedItem = eventData.pointerDrag?.GetComponent<ObjectButton>();

        if (draggedItem != null && manager != null)
        {
            // Mettre à jour la sélection dans le manager
            manager.ShowItemDetails(draggedItem.GetData());

            // Équiper dans la main appropriée
            if (isLeftHand)
                manager.PutInLeftHand();
            else
                manager.PutInRightHand();

            Debug.Log($"Objet déposé dans la main {(isLeftHand ? "Gauche" : "Droite")}");
        }
    }
}