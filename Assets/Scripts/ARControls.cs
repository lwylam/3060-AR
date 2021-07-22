using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class ARControls : MonoBehaviour
{
    public Slider Slide1, Slide2, Slide3;
    public GameObject Jnt1, Jnt2, Jnt3;
    public LineRenderer lineRenderer;
    public Text infoTxt;

    //private bool camAvailable;
    //private WebCamTexture backCam;
    //private Texture defaultBg;
    //public RawImage bg;
    //public AspectRatioFitter fit;

    private GameObject[] frm = new GameObject[5];
    private GameObject ptHead;
    private static readonly HttpClient client = new HttpClient(); // For cuter communication

    private void Start()
    {
        // Get the coordinate frame model objects
        for (int i = 0; i < 5; i++)
        {
            string frmName = "Frm" + i.ToString();
            frm[i] = GameObject.Find(frmName);
        }
        ptHead = GameObject.Find("point_head");

        //defaultBg = bg.texture;
        //WebCamDevice[] devices = WebCamTexture.devices;
        //if(devices.Length == 0){
        //    Debug.Log("No camera detected");
        //    camAvailable = false;
        //    return;
        //}

        //for(int i = 0; i < devices.Length; i++){
        //    if(!devices[i].isFrontFacing){ backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height); }
        //}
        //if(backCam == null){ Debug.Log("Unable to find back camera"); return; }
        //backCam.Play();
        //bg.texture = backCam;
        //Debug.Log("Camera found!!");
        //camAvailable = true;

        // if(backCam == null){ backCam = new WebCamTexture(); }
        // GetComponent<Renderer>().material.mainTexture = backCam;
        // if(!backCam.isPlaying){ backCam.Play(); }

        // lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        //lineRenderer.startWidth = 0.002f;
        //lineRenderer.endWidth = 0.002f;
    }

    void Update()
    {
        Vector3 origin = frm[0].transform.position;
        Vector3 end = frm[4].transform.position;
        Vector3 subEnd = end - Vector3.Normalize(end - origin) * 0.015f;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, subEnd);

        ptHead.transform.SetPositionAndRotation(end, Quaternion.LookRotation(end - origin));

        //if (!camAvailable){ return; }
        //float ratio = (float)backCam.width / (float)backCam.height;
        //fit.aspectRatio = ratio;
        //float scaleY = backCam.videoVerticallyMirrored? -1f:1f;
        //bg.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        //int orient = -backCam.videoRotationAngle;
        //bg.rectTransform.localEulerAngles = new Vector3(0,0,orient);
    }

    public void OnBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnUpdateJoint()
    {
        float j1 = Slide1.value;
        float j2 = Slide2.value;
        float j3 = Slide3.value;

        infoTxt.text = $"J1: {j1:0.0};	J2: {j2:0.0};	J3: {j3:0.0}";

        Jnt1.transform.localRotation = Quaternion.Euler(0, 180 - j1, 0);
        Jnt2.transform.localRotation = Quaternion.Euler(j2, -90, 0);
        Jnt3.transform.localRotation = Quaternion.Euler(j3 - 90, 0, 0);
    }

    public async void OnRequestJoint()
    {
        // read angle from http server
        var values = new Dictionary<string, string>
        {
            { "armID", "1" }
        };
        FormUrlEncodedContent content = new FormUrlEncodedContent(values);
        try
        {
            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1/readAngle", content); // url for real http server: "http://maeg3060.mae.cuhk.edu.hk/readAngle"
            string responseString = await response.Content.ReadAsStringAsync(); // responseString should be "100,150,50"

            char[] delim = { ',' };
            int[] j = responseString.Split(delim).Select(s => int.Parse(s)).ToArray();

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


    public void OnUpdateFrmScale()
    {
        Slider scaleSlider = GameObject.Find("ScaleSlider").GetComponent(typeof(Slider)) as Slider;
        float scale = scaleSlider.value;
        foreach (GameObject f in frm)
        {
            f.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void ShowDeviceList()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        infoTxt.text = $"Devices: {devices.Length}";
        foreach (WebCamDevice device in devices)
        {
            infoTxt.text += "\n" + device.name;
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
