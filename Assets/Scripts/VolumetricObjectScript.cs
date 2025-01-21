using UnityEngine;

public class VolumetricObjectScript : MonoBehaviour
{
    public GameObject SoldierObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshFilter meshFilter = SoldierObject.GetComponent<MeshFilter>();
        Debug.Log(meshFilter.mesh.name + "***********");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
