using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceManager : MonoBehaviour
{
    [Header("Furnace Parameters")]
    [SerializeField] private float temperature;
    private float range = 3.5f;

    [Header("Depencies")]
    public PlayerController player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Container furnaceContainer;

    [Header("Raycast Parameters")]
    private float lookDistance = 3f;

    [Header("UI Dependencies")]
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private Canvas FurnaceCanva;




    private void Start()
    {
        if (temperatureSlider != null && FurnaceCanva != null)
        {
            temperatureSlider.value = temperature;

            temperatureSlider.onValueChanged.AddListener(OnTemperatureSliderChanged);
        }
    }
    private void Update()
    {
        if (RangeCheck() && LookCheck())
        {
            FurnaceCanva.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            if (temperatureDisplay != null)
            {
                temperatureDisplay.text = "Temp: " + temperature.ToString("F0") + "°C";
            }
        }
        else
        {
            FurnaceCanva.gameObject.SetActive(false);

        }
    }

    private bool RangeCheck()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            return true;
        }
        return false;
    }

    private bool LookCheck()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            if (hit.transform.gameObject == this.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTemperatureSliderChanged(float newTemperature)
    {
        temperature = newTemperature;

        if (temperatureDisplay != null)
        {
            temperatureDisplay.text = "Temp :" + temperature.ToString("F0") + "°C";
        }

        Debug.Log("Changement de température du four");
    }

    public void Cooking()
    {
        if (furnaceContainer.ingredients.Count > 1 || furnaceContainer.ingredients.Count == 0)
        {
            Debug.Log("Trop d'ingrédients sur le grill");
        }
        else if (furnaceContainer.ingredients.Count == 1)
        {
            furnaceContainer.ingredients[0].isCooked = true;
        }
    }
}
