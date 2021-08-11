using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class ARBasics : MonoBehaviour
{
    public Text infoTxt;
    public GameObject Jnt1, Jnt2, Jnt3;

    private static readonly HttpClient client = new HttpClient(); // For cuter communication
    private int armId = 0;

    private void Start()
    {
        // Get saved arm ID
        if (PlayerPrefs.HasKey("ArmID"))
        {
            armId = PlayerPrefs.GetInt("ArmID");
            Debug.Log($"Saved arm ID loaded: {armId+1}");
        }
    }

    void FixedUpdate()
    {
        OnRequestJoint();
    }

    public void OnBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public async void OnRequestJoint()
    {
        // read angle from http server
        var values = new Dictionary<string, string>
        {
            { "armID", (armId+1).ToString() }
        };
        FormUrlEncodedContent content = new FormUrlEncodedContent(values);
        try
        {
            HttpResponseMessage response = await client.PostAsync("http://maeg3060.mae.cuhk.edu.hk/readAngle", content); // url for real http server: "http://maeg3060.mae.cuhk.edu.hk/readAngle"
            string responseString = await response.Content.ReadAsStringAsync(); // responseString should be "100,150,50"

            //infoTxt.text = responseString;
            char[] delim = { ',' };
            int[] j = responseString.Split(delim).Select(s => System.Convert.ToInt32(double.Parse(s))).ToArray();

            infoTxt.text = $"J1: {j[0]};	J2: {j[1]};	J3: {j[2]}";

            Jnt1.transform.localRotation = Quaternion.Euler(0, 180 - j[0], 0);
            Jnt2.transform.localRotation = Quaternion.Euler(j[1], -90, 0);
            Jnt3.transform.localRotation = Quaternion.Euler(j[2] - 90, 0, 0);
        }
        catch (System.Exception e)
        {
            infoTxt.text = $"Error msg:{e.Message}";
            Debug.Log(e.Message);
        }
    }

    public void ResetDeviceTracker()
    {
        var objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (objTracker != null && objTracker.IsActive)
        {
            Debug.Log("Stopping the ObjectTracker...");
            objTracker.Stop();

            // Create a temporary list of active datasets to prevent
            // InvalidOperationException caused by modifying the active
            // dataset list while iterating through it
            List<DataSet> activeDataSets = objTracker.GetActiveDataSets().ToList();

            // Reset active datasets
            foreach (DataSet dataset in activeDataSets)
            {
                objTracker.DeactivateDataSet(dataset);
                objTracker.ActivateDataSet(dataset);
            }

            Debug.Log("Restarting the ObjectTracker...");
            objTracker.Start();
        }

        var deviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        if (deviceTracker != null && deviceTracker.Reset()) { Debug.Log("Successfully reset device tracker"); }
        else { Debug.LogError("Failed to reset device tracker"); }
    }
}
