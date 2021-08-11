using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private TMP_Dropdown ArmIdDropdown;

    private void Start()
    {
        ArmIdDropdown = GameObject.Find("ArmID").GetComponent(typeof(TMP_Dropdown)) as TMP_Dropdown;
        if (PlayerPrefs.HasKey("ArmID"))
        {
            int id = PlayerPrefs.GetInt("ArmID");
            Debug.Log("Saved arm ID loaded");

            if (ArmIdDropdown != null)
            {
                ArmIdDropdown.value = id;
            }
        }
    }

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

    public void SaveArmID()
    {
        //Dropdown ArmIdDropdown = GameObject.Find("ArmID").GetComponent(typeof(Dropdown)) as Dropdown;
        int id = ArmIdDropdown.value;
        PlayerPrefs.SetInt("ArmID", id);
        PlayerPrefs.Save();
        Debug.Log("ArmID updated");
    }
    
    public void QuitApp()
    {
        Debug.Log("Quit button clicked");
        Application.Quit();
    }
}
