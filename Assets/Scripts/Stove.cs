using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stove : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiCanvas;
    public Slider temperatureSlider;
    public TextMeshProUGUI temperatureText;

    [Header("Settings")]
    public float maxDistance = 3f;

    private float currentTemp;
    private bool isInterfaceOpen = false;
    private Transform playerTransform;

    void Start()
    {
        uiCanvas.SetActive(false);

        temperatureSlider.onValueChanged.AddListener(OnSliderChanged);

        OnSliderChanged(temperatureSlider.value);
    }

    void Update()
    {
        if (isInterfaceOpen && playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist > maxDistance)
            {
                CloseInterface();
            }
        }
    }

    public void ToggleInterface(Transform player)
    {
        if (isInterfaceOpen)
        {
            CloseInterface();
        }
        else
        {
            OpenInterface(player);
        }
    }

    void OpenInterface(Transform player)
    {
        isInterfaceOpen = true;
        playerTransform = player;
        uiCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void CloseInterface()
    {
        isInterfaceOpen = false;
        uiCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnSliderChanged(float value)
    {
        currentTemp = value;
        temperatureSlider.minValue = 0;
        temperatureSlider.maxValue = 300;
        value = Mathf.Clamp(value, temperatureSlider.minValue, temperatureSlider.maxValue);
        temperatureText.text = Mathf.RoundToInt(currentTemp) + "°C";
    }
}