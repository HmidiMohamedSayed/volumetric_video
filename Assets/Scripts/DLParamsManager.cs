using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DLParamsManager : MonoBehaviour
{

    public GameObject UIToHide;
    public GameObject LoadingUI;
    public GameObject Pivot;
    public GameObject DirectionalLight;
    public GameObject PLParamsUI;
    private Light DLLight;
    public TMP_InputField RotationXInputField;
    public TMP_InputField RotationYInputField;
    public TMP_InputField RotationZInputField;
    public TMP_InputField DLIntensityInputField;
    public Toggle DLToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DLLight = DirectionalLight.GetComponent<Light>();
        RotationXInputField.text = DirectionalLight.transform.eulerAngles.x + "";
        RotationYInputField.text = DirectionalLight.transform.eulerAngles.y + "";
        RotationZInputField.text = DirectionalLight.transform.eulerAngles.z + "";
        DLIntensityInputField.text = DLLight.intensity + "";
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
            if (child.gameObject != this.gameObject && child.gameObject != LoadingUI && child.gameObject != PLParamsUI)
            {
                child.gameObject.SetActive(true);
            }
        }
        this.gameObject.SetActive(false);
        Pivot.transform.position = new Vector3(3, 0, 1.5f);
        Pivot.transform.eulerAngles = new Vector3(0, 10, 0);
    }


    public void RotationXInputFieldValueChanged()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (Regex.IsMatch(RotationXInputField.text, @"[\-\+]?[0-9]*(\.[0-9]+)?"))
        {
            try
            {
                float rotation = float.Parse(RotationXInputField.text, CultureInfo.InvariantCulture);
                DirectionalLight.transform.eulerAngles = new Vector3(rotation, DirectionalLight.transform.eulerAngles.y, DirectionalLight.transform.eulerAngles.z);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {
            RotationXInputField.text = "12";
            DirectionalLight.transform.eulerAngles = new Vector3(12, DirectionalLight.transform.eulerAngles.y, DirectionalLight.transform.eulerAngles.z);
        }
    }


    public void RotationYInputFieldValueChanged()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (Regex.IsMatch(RotationYInputField.text, @"[\-\+]?[0-9]*(\.[0-9]+)?"))
        {
            try
            {
                float rotation = float.Parse(RotationYInputField.text, CultureInfo.InvariantCulture);
                DirectionalLight.transform.eulerAngles = new Vector3(DirectionalLight.transform.eulerAngles.x, rotation,  DirectionalLight.transform.eulerAngles.z);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {
            RotationYInputField.text = "30";
            DirectionalLight.transform.eulerAngles = new Vector3(DirectionalLight.transform.eulerAngles.x, 30,  DirectionalLight.transform.eulerAngles.z);
        }
    }


    public void RotationZInputFieldValueChanged()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (Regex.IsMatch(RotationZInputField.text, @"[\-\+]?[0-9]*(\.[0-9]+)?"))
        {
            try
            {
                float rotation = float.Parse(RotationZInputField.text, CultureInfo.InvariantCulture);
                DirectionalLight.transform.eulerAngles = new Vector3(DirectionalLight.transform.eulerAngles.x, DirectionalLight.transform.eulerAngles.y, rotation);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {
            RotationZInputField.text = "0";
            DirectionalLight.transform.eulerAngles = new Vector3(DirectionalLight.transform.eulerAngles.x, DirectionalLight.transform.eulerAngles.y, 0);
        }
    }

    public void IntensityInputFieldValueChanged()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (Regex.IsMatch(DLIntensityInputField.text, @"[\-\+]?[0-9]*(\.[0-9]+)?"))
        {
            try
            {
                float intensity = float.Parse(DLIntensityInputField.text, CultureInfo.InvariantCulture);
                if (intensity > 5)
                {
                    intensity = 5;
                }
                else if (intensity < 0)
                {
                    intensity = 0;
                }
                DLLight.intensity = intensity;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {
            DLIntensityInputField.text = "0.85";
            DLLight.intensity = 0.85f;
        }
    }


    public void DLIsOnValueChanged()
    {
        DirectionalLight.SetActive(DLToggle.isOn);
    }

}
