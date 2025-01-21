using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
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

    //color picker params
    public GameObject ColorPicker;



    public GameObject UIToHide;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialPosition = Camera.transform.localPosition;
        initialPivotRotation = Pivot.transform.rotation;
        shader = ObjectLoaded.GetComponent<MeshRenderer>();
        UpdatePointSize();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseWheelTranslation();
        HandleObjectRotation();
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


    public void HandleColorPickerVisibility()
    {
       ColorPicker.SetActive(!ColorPicker.activeSelf);
    }


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



    public void OpenFileExplorer()
    {
        PathToLoad = SFB.StandaloneFileBrowser.OpenFilePanel("Choose a PLY file (.ply)", "", "ply",false)[0];
        Debug.Log(PathToLoad);
        if(PathToLoad.Length > 0)
        {
            LoadObject(PathToLoad);
        }

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
                MeshFilter meshFilter = ObjectLoaded.GetComponent<MeshFilter>();

                meshFilter.mesh = mesh;
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




    //public void AdjustLightIntensity()
    //{
    //    LightSource.intensity = intensity;
    //}
}
