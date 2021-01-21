using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionScript : MonoBehaviour
{
    public Text versionDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        versionDisplay = GameObject.Find("VersionText").GetComponent<Text>();
        if (versionDisplay != null)
        {
            versionDisplay.text = "Version: " + Application.version;
        }
        else
        {
            Debug.Log("Cant find gameobject of type Text with name VersionText");
        }
    }
}
