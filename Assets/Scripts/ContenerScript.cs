using System.Collections.Generic;
using UnityEngine;

public class ContenerScript : MonoBehaviour
{
    [Header("Camera")]
    private Camera cam;

    [Header("RayParam")]
    public float raydistance = 3f;

    [Header("Debug")]
    public GameObject current;     // Objet actuellement visé
    public GameObject last;        // Dernier objet visé

    public Canvas containerCanva;
    private PickUpController PUController;

    [Header("Recipe")]
    private Container targetContainer;

    void Start()
    {
        cam = Camera.main;
        PUController = GetComponent<PickUpController>();
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, raydistance))
        {
            GameObject target = hit.collider.gameObject;

            if (target.layer == LayerMask.NameToLayer("Container") && PUController._holding == true)
            {
                targetContainer = target.GetComponent<Container>();
                if (targetContainer == null) Debug.LogError("Alerte y'a pas de Container là"); // sécurité

                if (last != null & last != target)
                    DisableOutline(last);

                current = target;
                EnableOutline(current);
                last = current;
                containerCanva.enabled = true;
                containerCanva.transform.position = hit.collider.transform.position + Vector3.up;

                if (Input.GetMouseButtonDown(0) & PUController.leftHand != null)
                {
                    PUController.leftHand.transform.position = target.transform.position;
                    if (target.GetComponent<FurnaceManager>() != null)
                    {
                        Debug.Log("Ingredient Placé sur le four");
                        PUController.leftHand.transform.position += new Vector3(0, 1.5f, 0);
                    }
                    targetContainer.ingredientsContain.Add(PUController.leftHand);
                    PUController.leftHand.GetComponent<Ingredients>().currentContainer = targetContainer;
                    targetContainer.RefreshTypes();

                    PUController.leftHand = null;
                }

                if (Input.GetMouseButtonDown(1) & PUController.rightHand != null)
                {
                    PUController.rightHand.transform.position = target.transform.position;
                    if (target.GetComponent<FurnaceManager>() != null)
                    {
                        Debug.Log("Ingredient Placé sur le four");
                        PUController.rightHand.transform.position += new Vector3(0, 1.5f, 0);
                    }
                    targetContainer.ingredientsContain.Add(PUController.rightHand);
                    PUController.rightHand.GetComponent<Ingredients>().currentContainer = targetContainer;
                    targetContainer.RefreshTypes();
                    PUController.rightHand = null;
                }
                return;
            }
            


        }
        // Si on ne vise rien ou non-interactable, on nettoie proprement
        ClearOutline();
        containerCanva.enabled = false;

    }

    // Doublon des fonctions dans PickUp.cs / might want to create a common file ?
    void DisableOutline(GameObject obj)
    {
        if (obj == null) return;

        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }

    void EnableOutline(GameObject obj)
    {
        if (obj == null) return;

        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = true;
    }

    void ClearOutline()
    {
        if (last != null)
        {
            DisableOutline(last);
            last = null;
        }

        current = null;
    }
}
