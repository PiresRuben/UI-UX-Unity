using UnityEngine;
using UnityEngine.UI;

public class ObjectMenuController : MonoBehaviour
{
    private GameObject targetObject;

    public static Transform leftHandAnchor;
    public static Transform rightHandAnchor;

    private int uiLayer;

    private void Awake()
    {
        uiLayer = LayerMask.NameToLayer("UI_3D");
    }

    public void Initialize(GameObject objHit)
    {
        targetObject = objHit;
    }

    public void OnClickLeftHand()
    {
        EquipObject(leftHandAnchor);
        Destroy(gameObject);
    }

    public void OnClickRightHand()
    {
        EquipObject(rightHandAnchor);
        Destroy(gameObject);
    }

    private void EquipObject(Transform anchor)
    {
        if (targetObject == null || anchor == null) return;

        foreach (Transform child in anchor)
        {
            Destroy(child.gameObject);
        }

        GameObject clone = Instantiate(targetObject, anchor);

        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;

        SetLayerRecursively(clone, uiLayer);

        Destroy(clone.GetComponent<Rigidbody>());
        Destroy(clone.GetComponent<Collider>());
        Destroy(clone.GetComponent<Outline>());

        Debug.Log("Objet équipé sur : " + anchor.name);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}