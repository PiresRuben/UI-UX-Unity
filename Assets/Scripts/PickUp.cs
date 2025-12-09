using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 3f;

    [Header("Debug")]
    public GameObject current;     // Objet actuellement visé
    public GameObject last;        // Dernier objet visé

    private Camera cam;

    void Start()
    {
        cam = Camera.main; // Récupération caméra FPS
    }

    void Update()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * rayDistance, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance))
        {
            GameObject target = hit.collider.gameObject;
            Debug.Log(target.layer);
            if (target.layer == LayerMask.NameToLayer("Interactable")) // Layer Interactable
            {
                Debug.Log("Test2");
                // Désactiver l’outline de l'ancien si on change d’objet
                if (last != null && last != target)
                    DisableOutline(last);


                Debug.Log("Test3");
                current = target;
                EnableOutline(current);
                last = current;

                // Clic droit détruire l’objet
                if (Input.GetMouseButtonDown(1))
                {
                    DisableOutline(current);
                    Destroy(current);

                    current = null;
                    last = null;
                }

                return; // On évite d'appeler ClearOutline()
            }
        }

        // Si on ne vise rien ou non-interactable, on nettoie proprement
        ClearOutline();
    }

    void EnableOutline(GameObject obj)
    {
        if (obj == null) return;

        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = true;
    }

    void DisableOutline(GameObject obj)
    {
        if (obj == null) return;

        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
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
