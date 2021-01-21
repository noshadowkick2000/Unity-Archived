using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLight : MonoBehaviour
{
    private InGameTime _inGameTime;
    
    [SerializeField] private GameObject sun;
    [SerializeField] private Material windowMaterial;
    private Projector[] _simpleShadows;
    
    // Start is called before the first frame update
    void Awake()
    {
        _inGameTime = FindObjectOfType<InGameTime>();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("simpleshadow");
        _simpleShadows = new Projector[temp.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            _simpleShadows[i] = temp[i].GetComponent<Projector>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float t = (float) _inGameTime.GetTime() / Constants.minutesInDay;
        SetSun(t);
        SetSimpleShadows(t);
    }

    void SetSun(float t)
    {
        if (_inGameTime.GetTime()%30 == 0)
            sun.transform.rotation = Quaternion.RotateTowards(sun.transform.rotation, Quaternion.Euler(t*360-90, -50, 0), 1);
    }

    void SetSimpleShadows(float t)
    {
        foreach (var projector in _simpleShadows)
        {
            projector.farClipPlane = (t*4)+6;
        }
    }

    void SetWindows(float t)
    {
        
    }
}
