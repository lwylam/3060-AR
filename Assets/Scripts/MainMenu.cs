using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartAR()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartWorkspace()
    {
        SceneManager.LoadScene("ARWorkSpace");
    }

    public void StartMADCam()
    {
        SceneManager.LoadScene("MADCamera");
    }

    public void QuitApp()
    {
        Debug.Log("Quit button clicked");
        Application.Quit();
    }
}
