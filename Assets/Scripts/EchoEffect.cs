using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EchoEffect : MonoBehaviour
{
    public GameObject trailObj, baseRef, echo;
    public Slider PathNodeSlider;
    
    private float timeBtwSpawns = 0.4f;
    private float timeLeft;

    // Update is called once per frame
    void Update()
    {
        if (trailObj.activeSelf)// only if trail is enabled
        {
            if (timeLeft <= 0)
            {
                // spwan echo game object
                GameObject echoObj = (GameObject)Instantiate(echo, transform.position, Quaternion.identity);
                echoObj.transform.SetParent(baseRef.transform);
                Destroy(echoObj, 2f);
                timeLeft = timeBtwSpawns;
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }

    public void ChangePathNodeInterval()
    {
        timeBtwSpawns = PathNodeSlider.value;
    }
}