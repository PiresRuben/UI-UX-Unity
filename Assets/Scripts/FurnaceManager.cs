using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceManager : MonoBehaviour
{
    [Header("Furnace Parameters")]
    [SerializeField] private float temperature;
    [SerializeField] private float minCookingTemp = 80f;
    [SerializeField] private float targetCookingTime = 5f;
    [SerializeField] private float range = 3.5f;

    [Header("Dependencies")]
    public FPSController player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Container furnaceContainer;
    [SerializeField] private Button cookButton;

    [Header("Raycast Parameters")]
    [SerializeField] private float lookDistance = 3f;

    [Header("UI Dependencies")]
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private Canvas FurnaceCanva;

    private bool isCookingStarted = false;

    private void Start()
    {
        if (player == null) player = Object.FindFirstObjectByType<FPSController>();
        if (playerCamera == null) playerCamera = Camera.main;

        if (temperatureSlider != null)
        {
            temperatureSlider.value = temperature;
            temperatureSlider.onValueChanged.AddListener(OnTemperatureSliderChanged);
        }

        if (cookButton != null) cookButton.onClick.AddListener(StartCookingProcess);
        if (FurnaceCanva) FurnaceCanva.gameObject.SetActive(false);
    }

    public void StartCookingProcess()
    {
        if (furnaceContainer.ingredientsContain.Count > 0 && temperature >= minCookingTemp)
            isCookingStarted = true;
    }

    private void Update()
    {
        HandleUI();
        HandleCooking();
    }

    private void HandleCooking()
    {
        if (!isCookingStarted || temperature < minCookingTemp) return;
        if (furnaceContainer.ingredients.Count == 0) { isCookingStarted = false; return; }

        for (int i = 0; i < furnaceContainer.ingredients.Count; i++)
        {
            Ingredients ing = furnaceContainer.ingredients[i];
            if (ing != null && ing.canBeCooked && !ing.isCooked)
            {
                ing.cookingTimer += Time.deltaTime;
                if (ing.cookingTimer >= targetCookingTime) TransformSpawnCooked(i, ing);
            }
        }
    }

    private void TransformSpawnCooked(int index, Ingredients oldIng)
    {
        if (oldIng.cookedPrefab == null) return;

        Vector3 pos = oldIng.transform.position;
        Quaternion rot = oldIng.transform.rotation;
        Transform parent = oldIng.transform.parent;

        GameObject cookedObj = Instantiate(oldIng.cookedPrefab, pos, rot, parent);

        furnaceContainer.ingredientsContain[index] = cookedObj;
        Destroy(oldIng.gameObject);
        furnaceContainer.RefreshTypes();

        isCookingStarted = false;
    }

    private void HandleUI()
    {
        bool isLookingAtFurnace = RangeCheck() && LookCheck();
        if (isLookingAtFurnace && !FurnaceCanva.gameObject.activeSelf) FurnaceCanva.gameObject.SetActive(true);
        else if (!isLookingAtFurnace && FurnaceCanva.gameObject.activeSelf) FurnaceCanva.gameObject.SetActive(false);

        if (isLookingAtFurnace && temperatureDisplay != null)
            temperatureDisplay.text = "Temp: " + temperature.ToString("F0") + "°C";
    }

    private bool LookCheck()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out hit, lookDistance, LayerMask.GetMask("Stove", "Container")))
        {
            if (hit.transform.gameObject == this.gameObject) return true;
        }
        return false;
    }

    private bool RangeCheck() => player != null && Vector3.Distance(transform.position, player.transform.position) < range;

    private void OnTemperatureSliderChanged(float newTemperature) 
    { 
        temperature = newTemperature; 

        if (temperatureDisplay != null)
        {
            temperatureDisplay.text = "Temp: " + temperature.ToString("F0") + "°C";
        }
    }
}