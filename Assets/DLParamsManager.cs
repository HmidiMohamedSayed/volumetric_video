using UnityEngine;
using UnityEngine.UI;

public class DLParamsManager : MonoBehaviour
{

    public GameObject UIToHide;
    public GameObject LoadingUI;
    public GameObject Pivot;
    public GameObject DirectionalLight;
    private Light DLLight;
    public Slider RotationXSlider;
    public Slider RotationYSlider;
    public Slider RotationZSlider;
    public Slider DLIntensitySlider;
    public Toggle DLToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DLLight = DirectionalLight.GetComponent<Light>();
        RotationXSlider.value = DirectionalLight.transform.eulerAngles.x;
        RotationYSlider.value = DirectionalLight.transform.eulerAngles.y;
        RotationZSlider.value = DirectionalLight.transform.eulerAngles.z;
        DLIntensitySlider.value = DLLight.intensity;
        DLToggle.isOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitDLParamsWindow()
    {
        foreach (Transform child in UIToHide.transform)
        {
            if (child.gameObject != this.gameObject && child.gameObject != LoadingUI)
            {
                child.gameObject.SetActive(true);
            }
        }
        this.gameObject.SetActive(false);
        Pivot.transform.position = new Vector3(3, 0, 1.5f);
        Pivot.transform.eulerAngles = new Vector3(0, 10, 0);
    }


    public void RotationSliderValueChanged()
    {
        DirectionalLight.transform.eulerAngles = new Vector3(RotationXSlider.value, RotationYSlider.value, RotationZSlider.value);
    }

    public void IntensitySliderValueChanged()
    {
        DLLight.intensity = DLIntensitySlider.value;
    }


    public void DLIsOnValueChanged()
    {
        DirectionalLight.SetActive(DLToggle.isOn);
    }

}
