using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using SFB;
using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Linq; // Add this namespace for LINQ functions
using UnityEngine.Networking;

public class StaticUIManager2 : MonoBehaviour
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



    //point size parameters
    public TextMeshProUGUI PointSizeText;
    public static MeshRenderer shader;




    public GameObject UIToHide;





    public Slider LoadingBar;
    public TextMeshProUGUI LoadingText;
    private bool isLoading = false;
    public GameObject LoadingPanel;


    private string folderPath;


    private bool PanelReduced = false;
    public GameObject ReduceButton;
    public GameObject Panel;

    [DllImport("__Internal")]
    private static extern void OpenFolderBrowser();



    public GameObject DLParamsPanel;
    public GameObject PLParamsPanel;


    public GameObject LightBulb;

    public TMP_InputField IntensityInputField;
    public TMP_InputField PointSizeInputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        InitialPosition = Camera.transform.localPosition;
        initialPivotRotation = Pivot.transform.rotation;
        shader = ObjectLoaded.GetComponent<MeshRenderer>();
        Debug.Log(shader.material.GetFloat("_Ambient"));
        IntensityInputField.text = shader.material.GetFloat("_Ambient") + "";
        PointSizeInputField.text = shader.material.GetFloat("_PointSize") + "";
        UpdatePointSize();
        LoadingPanel.SetActive(false);
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


    public void PLParamWindowHandler()
    {
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject == PLParamsPanel)
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

    // Update is called once per frame
    void Update()
    {
        HandleMouseWheelTranslation();
        HandleObjectRotation();
    }

    Vector3 InitialLocalPosition = new Vector3();
    Vector3 InitialLocalRotation = new Vector3();


    public void DisableEnableUI(bool isActive)
    {
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject != LoadingPanel && child.gameObject != DLParamsPanel && child.gameObject != PLParamsPanel)
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }

    private void UpdatePointSize()
    {
        //PointSizeText.text = "Point Size : " + shader.material.GetFloat("_PointSize").ToString("F3");
        //PointSizeSilder.value = shader.material.GetFloat("_PointSize");
    }



    public void IntensityInputFieldValueChanged()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (Regex.IsMatch(IntensityInputField.text, @"[\-\+]?[0-9]*(\.[0-9]+)?"))
        {
            try
            {
                float intensity = float.Parse(IntensityInputField.text, CultureInfo.InvariantCulture);
                shader.material.SetFloat("_Ambient", intensity);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {
            IntensityInputField.text = "0.15";
            shader.material.SetFloat("_Ambient", 0.15f);
        }

    }



    public void PointSizeInputFieldValueChanged()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (Regex.IsMatch(PointSizeInputField.text, @"[\-\+]?[0-9]*(\.[0-9]+)?"))
        {
            try
            {
                float size = float.Parse(PointSizeInputField.text, CultureInfo.InvariantCulture);
                if(size > 5)
                {
                    size = 5;
                }
                else if(size < 0)
                {
                    size = 0;
                }
                shader.material.SetFloat("_PointSize",size);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {
            PointSizeInputField.text = "2";
            shader.material.SetFloat("_PointSize", 2);
        }

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
                UnityEngine.Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
                UnityEngine.Cursor.visible = false; // Hide the cursor
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            UnityEngine.Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            UnityEngine.Cursor.visible = true; // Show the cursor
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

    public void BrowseFolder()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                 OpenFolderBrowser();
#endif
    }

    public void ReceivePlyFileCount(int fileCount)
    {
        Debug.Log($"Number of .ply files: {fileCount}");
        PLYFilesloaded = 0;
        PLYFilesCount = fileCount;
        if(fileCount > 0)
        {
            EnDisableAllExceptLoadingPanel(false);
        }
    }

    int PLYFilesloaded = 0;
    int PLYFilesCount = 0;

    public void ReceivePlyFilePath(string fileName)
    {
        Debug.Log("Trying to load PLY full path file: " + fileName);
        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
        foreach(string file in files)
        {
            Debug.Log("file : " + file);
        }
        fileName = Path.Combine(Application.streamingAssetsPath, fileName);
        Debug.Log("ply full path : " + fileName);
    
        StartCoroutine(LoadPlyFile(fileName));
    }

    //public void ReceivePlyFileData(string jsonData)
    //{
    //    PlyFileData plyFile = JsonUtility.FromJson<PlyFileData>(jsonData);
    //    byte[] binaryData = plyFile.data.Select(b => (byte)b).ToArray();
    //    Debug.Log(plyFile.data);
    //    // Process the file (Load it into the mesh list)
    //    Mesh mesh = LoadPLYAsMeshWEB(binaryData);
    //    if (mesh != null)
    //    {
    //        Debug.Log($"Mesh is not null");
    //        MeshList.Add(mesh);
    //        PLYFilesloaded++;
    //        float progress = (float)PLYFilesloaded / PLYFilesCount;
    //        UpdateProgressBar(progress);
    //    }
    //    Debug.Log($"In ReceivePlyFileData line 523");
    //    if (PLYFilesloaded == PLYFilesCount)
    //    {
    //        FrameSlider.maxValue = MeshList.Count - 1; // Set max value to total frames
    //        FrameSlider.value = 0; // Start at the first frame
    //        if (MeshList.Count > 0)
    //        {
    //            UpdateMesh(0); // Display the first frame
    //            Debug.Log($"In UpdateMesh");
    //        }

    //        // Masquer la barre de chargement lorsque le chargement est termin�
    //        isLoading = false;
    //        LoadingPanel.SetActive(false);
    //        EnDisableAllExceptLoadingPanel(true);
    //    }
    //}

    public IEnumerator LoadPlyFile(string filePath)
    {
        // Process the file (Load it into the mesh list)

        // Ensure the URL format is correct
        if (!filePath.StartsWith("http"))
        {
            filePath = "http://" + filePath.TrimStart('/');
        }

        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            byte[] fileData = www.downloadHandler.data;
            Debug.Log("File successfully loaded from WebGL StreamingAssets: " + filePath);

            string directory = Directory.GetCurrentDirectory();
            // Write byte[] to file
            string tempFileFullPath = Path.Combine(directory, filePath.Split(@"/").Last());
            File.WriteAllBytes(tempFileFullPath, fileData);
            Mesh mesh = LoadPLYAsMesh(tempFileFullPath);
            if (mesh != null)
            {
                Debug.Log($"Mesh is not null");
                TempMesh = mesh;
                PLYFilesloaded++;
                float progress = (float)PLYFilesloaded / PLYFilesCount;
                UpdateProgressBar(progress);
            }
            Debug.Log($"In ReceivePlyFileData line 523");
            UpdateMesh(); // Display the first frame
            Debug.Log($"In UpdateMesh");

            // Masquer la barre de chargement lorsque le chargement est termin�
            isLoading = false;
            LoadingPanel.SetActive(false);
            EnDisableAllExceptLoadingPanel(true);
            // Process the data (e.g., parse the PLY file)
        }
        else
        {
            Debug.LogError("Failed to load file: " + www.error);
        }
        yield return null;
    }

    private Mesh LoadPLYAsMeshWEB(byte[] binaryData)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> indices = new List<int>();
        Debug.Log(binaryData.Count());

        using (MemoryStream memoryStream = new MemoryStream(binaryData))
        using (var reader = new BinaryReader(memoryStream))
        {
            // Parse the header
            bool headerEnded = false;
            bool hasNormals = false;
            while (!headerEnded)
            {
                string line = ReadAsciiLine(reader);
                if (line.StartsWith("property float nx"))
                    hasNormals = true;
                if (line.StartsWith("end_header"))
                    headerEnded = true;
            }

            // Parse vertex data
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();

                float nx = 0, ny = 0, nz = 0;
                if (hasNormals)
                {
                    nx = reader.ReadSingle();
                    ny = reader.ReadSingle();
                    nz = reader.ReadSingle();
                }

                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();

                vertices.Add(new Vector3(x, y, z));
                normals.Add(new Vector3(nx, ny, nz)); // Store normal
                colors.Add(new Color32(r, g, b, a));

                indices.Add(indices.Count); // Add sequential indices
            }
        }

        // Create and return the mesh
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Supports large meshes
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals); // Apply normals
        mesh.SetColors(colors);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0); // Use "Points" for visualization
        mesh.RecalculateBounds();

        return mesh;
    }

    public void OpenFileExplorer()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        BrowseFolder();
#else
        PathToLoad = SFB.StandaloneFileBrowser.OpenFilePanel("Choose a ply file", "", "ply", false)[0];
        Debug.Log("Selected file: " + PathToLoad);
        //#endif
        if (!string.IsNullOrEmpty(PathToLoad) && File.Exists(PathToLoad))
        {

            LoadingPanel.SetActive(true); // Afficher le panneau de chargement
            StartCoroutine(LoadFileWithProgress(PathToLoad)); // Utiliser une coroutine pour charger les fichiers
        }
        else
        {
            Debug.Log("File: " + PathToLoad + " doesn't exist");
        }
#endif

    }


    private IEnumerator LoadFileWithProgress(string plyFile)
    {
        isLoading = true;
        TempMesh = null;

        int totalFiles = 1;
        int loadedFiles = 0;
        if (File.Exists(plyFile))
        {
            // Charger le fichier PLY
            LoadObject(plyFile);
            EnDisableAllExceptLoadingPanel(false);
            // Mettre � jour la barre de progression
            loadedFiles++;
            float progress = (float)loadedFiles / totalFiles;
            UpdateProgressBar(progress);

            yield return null; // Permettre � l'UI de se rafra�chir entre chaque it�ration
        }
        UpdateMesh(); // Display the first frame

        // Masquer la barre de chargement lorsque le chargement est termin�
        isLoading = false;
        LoadingPanel.SetActive(false);
        EnDisableAllExceptLoadingPanel(true);
    }



    private void UpdateProgressBar(float progress)
    {
        LoadingBar.value = progress * 100;
        LoadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
    }


    private void UpdateMesh()
    {
        if (TempMesh != null)
        {
            MeshFilter meshFilter = ObjectLoaded.GetComponent<MeshFilter>();
            meshFilter.mesh = TempMesh;
            meshFilter.sharedMesh = TempMesh;
            Debug.Log("in here");
        }
    }

    private Mesh TempMesh;
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

                TempMesh = mesh;
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
        List<Vector3> normals = new List<Vector3>(); // Store normals
        List<Color> colors = new List<Color>();
        List<int> indices = new List<int>();
        filePath = filePath.Replace(@"/C", @"C");
        Debug.Log("the full path 2 is : " + filePath);

        if (File.Exists(filePath))
        {
            Debug.Log("the file exists and the full path 2 is : " + filePath);
        }

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            // Parse the header
            bool headerEnded = false;
            bool hasNormals = false;
            while (!headerEnded)
            {
                string line = ReadAsciiLine(reader);
                if (line.StartsWith("property float nx"))
                    hasNormals = true;
                if (line.StartsWith("end_header"))
                    headerEnded = true;
            }

            Debug.Log("hasNormals = " + hasNormals);
            Debug.Log("hasNormals = " + hasNormals);

            // Parse vertex data
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();

                float nx = 0, ny = 0, nz = 0;
                if (hasNormals)
                {
                    nx = reader.ReadSingle();
                    ny = reader.ReadSingle();
                    nz = reader.ReadSingle();
                }

                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();

                vertices.Add(new Vector3(x, y, z));
                normals.Add(new Vector3(nx, ny, nz)); // Store normal
                colors.Add(new Color32(r, g, b, a));

                indices.Add(indices.Count); // Add sequential indices
            }
        }

        // Create and return the mesh
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Supports large meshes
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals); // Apply normals
        mesh.SetColors(colors);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0); // Use "Points" for visualization
        mesh.RecalculateBounds();
        mesh.MarkDynamic();  // Important for WebGL performance

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
            // D�sactive l'enfant si ce n'est pas le LoadingPanel ni le DLParamsPanel ni le PLParamsPanel
            if (child.gameObject != LoadingPanel && child.gameObject != DLParamsPanel && child.gameObject != PLParamsPanel)
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
