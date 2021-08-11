using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EchoEffect : MonoBehaviour
{
    public GameObject trailObj;
    public GameObject echo;
    public float timeBtwSpawns;

    private float timeLeft;

    // Update is called once per frame
    void Update()
    {
        if (trailObj.activeSelf)// only if trail is enabled
        {
            if (timeLeft <= 0)
            {
                // spwan echo game object
                GameObject instace = (GameObject)Instantiate(echo, transform.position, Quaternion.identity);
                Destroy(instace, 2f);
                timeLeft = timeBtwSpawns;
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}