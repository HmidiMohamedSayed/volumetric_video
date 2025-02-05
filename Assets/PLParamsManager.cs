using UnityEngine;
using UnityEngine.UI;

public class PLParamsManager : MonoBehaviour
{

    public GameObject UIToHide;
    public GameObject LoadingUI;
    public GameObject DLParamsUI;
    public GameObject Pivot;
    public GameObject PointLight;
    public GameObject BulbObject;
    private Light PLLight;
    public Slider PositionXSlider;
    public Slider PositionYSlider;
    public Slider PositionZSlider;
    public Slider PLIntensitySlider;
    public Toggle PLToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PLLight = PointLight.GetComponent<Light>();
        PositionXSlider.value = BulbObject.transform.position.x;
        PositionYSlider.value = BulbObject.transform.position.y;
        PositionZSlider.value = BulbObject.transform.position.z;
        PLIntensitySlider.value = PLLight.intensity;
        PLToggle.isOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitPLParamsWindow()
    {
        foreach (Transform child in UIToHide.transform)
        {
            if (child.gameObject != this.gameObject && child.gameObject != LoadingUI && child.gameObject != DLParamsUI)
            {
                child.gameObject.SetActive(true);
            }
        }
        this.gameObject.SetActive(false);
        Pivot.transform.position = new Vector3(3, 0, 1.5f);
        Pivot.transform.eulerAngles = new Vector3(0, 10, 0);
    }


    public void PositionSliderValueChanged()
    {
        BulbObject.transform.position = new Vector3(PositionXSlider.value, PositionYSlider.value, PositionZSlider.value);
    }

    public void IntensitySliderValueChanged()
    {
        PLLight.intensity = PLIntensitySlider.value;
    }


    public void PLIsOnValueChanged()
    {
        PointLight.SetActive(PLToggle.isOn);
    }

}
