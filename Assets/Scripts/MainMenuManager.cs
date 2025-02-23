using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GoToMainMenuSceneScene()
    {
        SceneManager.LoadScene(0);
    }


    public void GoToStaticScene()
    {
        SceneManager.LoadScene(1);
    }



    public void GoToDynamicScene()
    {
        SceneManager.LoadScene(2);
    }
}
