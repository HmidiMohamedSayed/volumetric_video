using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using SFB;
public class DynamicUIManager : MonoBehaviour
{

    public GameObject Pivot;
    public GameObject Camera;
    public GameObject ObjectLoaded;
    public float TranslationScale = 1f;
    private Vector3 InitialPosition;
    private Quaternion initialPivotRotation;
    public float MinTranslation = -10f; // Minimum translation limit
    public float MaxTranslation = 10f;  // Maximum translation limit
    private float currentTranslation = 0f; // Track the current translation value
    private string PathToLoad;

    public float RotationSpeed = 100f; // Speed of rotation
    public float VerticalRotationLimit = 45f; // Limit for vertical rotation in degrees

    private bool isDragging = false; // Whether the user is dragging


    public Material DefaultMaterial; // Material to apply to the loaded object


    //point size parameters
    public Slider PointSizeSilder;
    public TextMeshProUGUI PointSizeText;
    public static MeshRenderer shader;




    public GameObject UIToHide;





    public Slider LoadingBar;
    public TextMeshProUGUI LoadingText;
    private bool isLoading = false;
    public GameObject LoadingPanel;


    public Slider FrameSlider; // Assign this in the Inspector
    private bool isPlaying = false;
    private int currentFrame = 0; // Track the current frame



    public string videoOutputPath = "RecordedVideo"; // Nom du dossier de sortie
    private bool isRecording = false; // Statut de l'enregistrement
    private float timeBetweenFrames;
    private int frameCount = 0; // Compteur de frames
    private string folderPath;


    private bool PanelReduced = false;
    public GameObject ReduceButton;
    public GameObject Panel;



    //frame rate parameters
    public float FrameRate = 30f;
    public Button FrameButton20;
    public Button FrameButton30;
    public Button FrameButton45;
    public Button FrameButton60;


    public Slider IntensitySilder;


    public GameObject DLParamsPanel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialPosition = Camera.transform.localPosition;
        initialPivotRotation = Pivot.transform.rotation;
        shader = ObjectLoaded.GetComponent<MeshRenderer>();
        UpdatePointSize();
        LoadingPanel.SetActive(false);
        folderPath = Path.Combine(@"C:\Users\mhi\Desktop\IDIA 5\PRED", videoOutputPath);
        FrameButton30.GetComponent<Image>().color = Color.grey;
    }


    public void FrameButton30Handler()
    {
        FrameButton30.GetComponent<Image>().color = Color.grey;
        FrameButton20.GetComponent<Image>().color = Color.white;
        FrameButton45.GetComponent<Image>().color = Color.white;
        FrameButton60.GetComponent<Image>().color = Color.white;
        FrameRate = 30f;
        timeBetweenFrames = 1f / FrameRate; // Calcul du délai entre les frames
    }


    public void DLParamWindowHandler()
    {
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject == DLParamsPanel)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
        Pivot.transform.position = new Vector3(0, 0, 0);
        Pivot.transform.eulerAngles = new Vector3(0, 0, 0);

    }


    public void FrameButton20Handler()
    {
        FrameButton30.GetComponent<Image>().color = Color.white;
        FrameButton20.GetComponent<Image>().color = Color.grey;
        FrameButton45.GetComponent<Image>().color = Color.white;
        FrameButton60.GetComponent<Image>().color = Color.white;
        FrameRate = 20f;
        timeBetweenFrames = 1f / FrameRate; // Calcul du délai entre les frames
    }


    public void FrameButton45Handler()
    {
        FrameButton30.GetComponent<Image>().color = Color.white;
        FrameButton20.GetComponent<Image>().color = Color.white;
        FrameButton45.GetComponent<Image>().color = Color.grey;
        FrameButton60.GetComponent<Image>().color = Color.white;
        FrameRate = 45f;
        timeBetweenFrames = 1f / FrameRate; // Calcul du délai entre les frames
    }


    public void FrameButton60Handler()
    {
        FrameButton30.GetComponent<Image>().color = Color.white;
        FrameButton20.GetComponent<Image>().color = Color.white;
        FrameButton45.GetComponent<Image>().color = Color.white;
        FrameButton60.GetComponent<Image>().color = Color.grey;
        FrameRate = 60f;
        timeBetweenFrames = 1f / FrameRate; // Calcul du délai entre les frames
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseWheelTranslation();
        HandleObjectRotation();
        // Arrêter l'enregistrement en appuyant sur la touche Espace
        if (isRecording && Input.GetKeyDown(KeyCode.Space))
        {
            StopRecording();
        }
    }

    Vector3 InitialLocalPosition = new Vector3();
    Vector3 InitialLocalRotation = new Vector3();
    public void StartRecording()
    {
        if (!isRecording)
        {
            InitialLocalPosition = Pivot.transform.position;
            InitialLocalRotation = Pivot.transform.eulerAngles;
            Pivot.transform.position = new Vector3(0, 0, 0);
            Pivot.transform.eulerAngles = new Vector3(0, 0, 0);
            isRecording = true;
            frameCount = 0;
            currentFrame = 0;
            isPlaying = false;
            timeBetweenFrames = 1f / FrameRate; // Calcul du délai entre les frames
            PlayHandler();
            DisableEnableUI(false);
            // Crée un dossier temporaire pour stocker les frames
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }

            Directory.CreateDirectory(folderPath);

            Debug.Log("Recording started...");
            StartCoroutine(CaptureFrames());
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            Debug.Log("Recording stopped. Frames saved in: " + Path.GetFullPath(folderPath));
            ConvertToVideo();
            DisableEnableUI(true);
            Pivot.transform.position = InitialLocalPosition;
            Pivot.transform.eulerAngles = InitialLocalRotation;

        }

    }

    private IEnumerator CaptureFrames()
    {
        while (isRecording)
        {
            yield return new WaitForSeconds(timeBetweenFrames);

            // Capture et sauvegarde chaque frame comme image PNG
            string filePath = Path.Combine(folderPath, $"frame_{frameCount:D4}.png");
            ScreenCapture.CaptureScreenshot(filePath);
            frameCount++;

            Debug.Log("Captured frame: " + filePath);

            if (frameCount > MeshList.Count)
            {
                StopRecording();
            }

        }
    }

    public void ToggleRecording()
    {
        if (isRecording)
            StopRecording();
        else
            StartRecording();
    }


    private void ConvertToVideo()
    {
        string ffmpegPath = @"C:\Users\mhi\AppData\Local\Microsoft\WinGet\Packages\Gyan.FFmpeg_Microsoft.Winget.Source_8wekyb3d8bbwe\ffmpeg-7.1-full_build\bin\ffmpeg.exe";
        string outputVideo = Path.Combine(folderPath, "output.mp4");

        string arguments = $"-y -r {FrameRate} -i \"{folderPath}/frame_%04d.png\" -s 1920x1080 -vcodec libx264 -pix_fmt yuv420p \"{outputVideo}\"";
        System.Diagnostics.Process ffmpegProcess = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        ffmpegProcess.Start();

        // Lire les erreurs éventuelles
        string error = ffmpegProcess.StandardError.ReadToEnd();
        ffmpegProcess.WaitForExit();

        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("FFmpeg Error: " + error);
        }
        else
        {
            Debug.Log("Video successfully created: " + outputVideo);
        }
        Debug.Log("Video saved to: " + outputVideo);
    }


    public void DisableEnableUI(bool isActive)
    {
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject != LoadingPanel)
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }

    private void UpdatePointSize()
    {
        PointSizeText.text = "Point Size : " + shader.material.GetFloat("_PointSize").ToString("F3");
        PointSizeSilder.value = shader.material.GetFloat("_PointSize");
    }


    public void PointSizeSliderValueChanged()
    {
        PointSizeText.text = "Point Size : " + PointSizeSilder.value.ToString("F3");
        shader.material.SetFloat("_PointSize", PointSizeSilder.value);
    }



    public void IntensitySliderValueChanged()
    {
        shader.material.SetFloat("_Ambient", IntensitySilder.value);
    }



    //public void HandleColorPickerVisibility()
    //{
    //   ColorPicker.SetActive(!ColorPicker.activeSelf);
    //}


    private void HandleObjectRotation()
    {
        // Detect mouse button press/release
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse is over a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                isDragging = true;
                Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
                Cursor.visible = false; // Hide the cursor
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Show the cursor
        }

        // Rotate the object while dragging
        if (isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X") * 2.5f; // Horizontal mouse movement
            float mouseY = -1 * Input.GetAxis("Mouse Y") * 2.5f; // Vertical mouse movement

            // Horizontal rotation (around the Y-axis)
            float horizontalRotation = mouseX * RotationSpeed * Time.deltaTime;
            Pivot.transform.Rotate(Vector3.up, -horizontalRotation, Space.World);

            // Vertical rotation (around the X-axis)
            float verticalRotation = mouseY * RotationSpeed * Time.deltaTime;
            float currentXAngle = Pivot.transform.eulerAngles.x;

            // Clamp vertical rotation to limit
            float newXAngle = currentXAngle - verticalRotation;
            newXAngle = ClampAngle(newXAngle, -VerticalRotationLimit, VerticalRotationLimit);

            // Apply clamped vertical rotation
            Pivot.transform.eulerAngles = new Vector3(
                newXAngle,
                Pivot.transform.eulerAngles.y,
                Pivot.transform.eulerAngles.z
            );
        }
    }

    // Helper function to clamp an angle between min and max
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }


    void HandleMouseWheelTranslation()
    {
        // Get the mouse wheel input for translation
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Update the translation amount, clamped to the specified limits
        currentTranslation = Mathf.Clamp(
            currentTranslation + scrollInput * TranslationScale,
            MinTranslation,
            MaxTranslation
        );

        // Apply the translation to the camera
        Camera.transform.localPosition = InitialPosition + Camera.transform.forward * currentTranslation;
    }


    List<Mesh> MeshList = new List<Mesh>();

    public void OpenFileExplorer()
    {

        PathToLoad = SFB.StandaloneFileBrowser.OpenFolderPanel("Choose a folder", "",false)[0];
        Debug.Log(PathToLoad);

        if (!string.IsNullOrEmpty(PathToLoad) && Directory.Exists(PathToLoad))
        {
            string[] PlyFiles = Directory.GetFiles(PathToLoad, "*.ply");
            if (PlyFiles.Length > 0)
            {
                LoadingPanel.SetActive(true); // Afficher le panneau de chargement
                StartCoroutine(LoadFilesWithProgress(PlyFiles)); // Utiliser une coroutine pour charger les fichiers
            }
        }
        else
        {
            Debug.Log("Directory: " + PathToLoad + " doesn't exist");
        }
    }


    private IEnumerator LoadFilesWithProgress(string[] plyFiles)
    {
        isLoading = true;
        MeshList.Clear();

        int totalFiles = plyFiles.Length;
        int loadedFiles = 0;

        foreach (string file in plyFiles)
        {
            if (File.Exists(file))
            {
                // Charger le fichier PLY
                LoadObject(file);
                EnDisableAllExceptLoadingPanel(false);
                // Mettre à jour la barre de progression
                loadedFiles++;
                float progress = (float)loadedFiles / totalFiles;
                UpdateProgressBar(progress);

                yield return null; // Permettre à l'UI de se rafraîchir entre chaque itération
            }
        }

        // Initialize the slider when loading is complete
        FrameSlider.maxValue = MeshList.Count - 1; // Set max value to total frames
        FrameSlider.value = 0; // Start at the first frame


        if (MeshList.Count > 0)
        {
            UpdateMesh(0); // Display the first frame
        }

        // Masquer la barre de chargement lorsque le chargement est terminé
        isLoading = false;
        LoadingPanel.SetActive(false);
        EnDisableAllExceptLoadingPanel(true);
    }



    private void UpdateProgressBar(float progress)
    {
        LoadingBar.value = progress * 100;
        LoadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
    }


    public void PlayHandler()
    {
        if(MeshList != null && MeshList.Count > 0 && !isPlaying)
        {
            isPlaying = true;
            StartCoroutine(PlayFrames());
        }
    }


    private IEnumerator PlayFrames()
    {
        float delay = 1f / FrameRate;
        while (isPlaying && currentFrame < MeshList.Count)
        {
            UpdateMesh(currentFrame);
            FrameSlider.value = currentFrame; // Update the slider
            currentFrame++;
            yield return new WaitForSeconds(delay);
        }
        isPlaying = false; // Stop playing when all frames are displayed
    }


    public void OnSliderValueChanged()
    {
        if (!isPlaying) // Allow seeking only when paused
        {
            currentFrame = Mathf.RoundToInt(FrameSlider.value); // Update current frame
            UpdateMesh(currentFrame); // Display the selected frame
        }
    }


    private void UpdateMesh(int frameIndex)
    {
        if (frameIndex >= 0 && frameIndex < MeshList.Count)
        {
            MeshFilter meshFilter = ObjectLoaded.GetComponent<MeshFilter>();
            meshFilter.mesh = MeshList[frameIndex];
        }
    }


    public void PauseHandler()
    {
        isPlaying = false; // Stop the playback
    }


    private void LoadObject(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File does not exist: " + filePath);
            return;
        }

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            Mesh mesh = LoadPLYAsMesh(filePath);
            if (mesh != null)
            {

                MeshList.Add(mesh);
                System.GC.Collect();
                Resources.UnloadUnusedAssets();
            }
        }
        else
        {
            Debug.LogError("File path is invalid or the file does not exist.");
        }
    }

    private Mesh LoadPLYAsMesh(string filePath)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> indices = new List<int>();

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            // Parse the header
            bool headerEnded = false;
            while (!headerEnded)
            {
                string line = ReadAsciiLine(reader);
                if (line.StartsWith("end_header"))
                {
                    headerEnded = true;
                }
            }

            // Parse vertex data
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();

                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();

                vertices.Add(new Vector3(x, y, z));
                colors.Add(new Color32(r, g, b, a));

                indices.Add(indices.Count); // Add sequential indices
            }
        }

        // Create and return the mesh
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Supports large meshes
        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0); // Use "Points" for visualization
        mesh.RecalculateBounds();

        return mesh;
    }

    private string ReadAsciiLine(BinaryReader reader)
    {
        List<byte> bytes = new List<byte>();
        byte b;
        while ((b = reader.ReadByte()) != 10) // ASCII newline
        {
            bytes.Add(b);
        }
        return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
    }

    public void ResetView()
    {
        currentTranslation = 0;
        Pivot.transform.rotation = initialPivotRotation;
    }

    public void EnDisableAllExceptLoadingPanel(bool activate)
    {
        // Parcours de tous les enfants de ParametersUI
        foreach (Transform child in this.gameObject.transform)
        {
            // Désactive l'enfant si ce n'est pas le LoadingPanel ni le DLParamsPanel
            if (child.gameObject != LoadingPanel && child.gameObject != DLParamsPanel)
            {
                child.gameObject.SetActive(activate);
            }
        }
        //ColorPicker.SetActive(false);
    }



    public void ResizePanel()
    {
        if (PanelReduced)
        {
            ReduceButton.transform.position += new Vector3(230, 0, 0);
            Panel.SetActive(true);
            Pivot.transform.position = new Vector3(3,0,1.5f);
            Pivot.transform.eulerAngles = new Vector3(0,10,0);
            Sprite newSprite = Resources.Load<Sprite>(@"Images/reduire");
            ReduceButton.GetComponent<Image>().sprite = newSprite;
        }
        else
        {
            ReduceButton.transform.position += new Vector3(-230, 0, 0);
            Panel.SetActive(false);
            Pivot.transform.position = new Vector3(0, 0, 0);
            Pivot.transform.eulerAngles = new Vector3(0, 0, 0);
            Sprite newSprite = Resources.Load<Sprite>(@"Images/maximize2");
            ReduceButton.GetComponent<Image>().sprite = newSprite;
        }
        PanelReduced = !PanelReduced;
    }

    //public void AdjustLightIntensity()
    //{
    //    LightSource.intensity = intensity;
    //}
}
