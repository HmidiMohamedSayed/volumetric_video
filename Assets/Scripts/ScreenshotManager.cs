using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class ScreenshotManager : MonoBehaviour
{
    public GameObject uiElement; // Reference to the UI element to hide
    public Image successIcon;
    private string screenshotsFolder = "ScreenShots";
    private AudioSource audioSource;

    public GameObject LightBulb;

    private void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject!");
        }
    }


    public void TakeScreenshot()
    {
        string folderPath = Path.Combine(Application.dataPath, screenshotsFolder);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        int fileCount = Directory.GetFiles(folderPath, "*.png").Length;

        string fileName = "ObjectView" + (fileCount + 1) + ".png";

        string filePath = Path.Combine(folderPath, fileName);

        StartCoroutine(CaptureScreenshotCoroutine(filePath));
    }

    private IEnumerator CaptureScreenshotCoroutine(string filePath)
    {
        // Deactivate the UI element
        if (uiElement != null)
        {
            uiElement.SetActive(false);
            foreach (Renderer renderer in LightBulb.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false; // or true to show
            }
        }


        // Capture the screenshot
        ScreenCapture.CaptureScreenshot(filePath);
        audioSource.Play();
        Debug.Log("Screenshot saved at: " + filePath);

        // Wait for the end of the frame to ensure the screenshot is taken
        yield return new WaitForEndOfFrame();

        // Reactivate the UI element
        if (uiElement != null)
        {
            uiElement.SetActive(true);
            foreach (Renderer renderer in LightBulb.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true; // or true to show
            }
        }

        // Optionally, show a success icon or perform additional actions
        StartCoroutine(ShowSuccessIcon());
    }

    private IEnumerator ShowSuccessIcon()
    {
        successIcon.enabled = true;
        yield return new WaitForSeconds(2);
        successIcon.enabled = false;
    }
}
