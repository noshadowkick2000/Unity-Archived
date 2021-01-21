using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameTime : MonoBehaviour
{
    private int _inGameTime;

    [SerializeField] private Text timeDisplayer;
    
    // Start is called before the first frame update
    void Start()
    {
        _inGameTime = PlayerPrefs.GetInt("InGameTime");

        StartCoroutine(RunClock());
    }

    private IEnumerator RunClock()
    {
        if (_inGameTime <= Constants.minutesInDay)
            _inGameTime++;
        else
            _inGameTime = 0;
        SetDisplay();
        yield return new WaitForSecondsRealtime(1);
        StartCoroutine(RunClock());
    }

    private void SetDisplay()
    {
        int minutes = _inGameTime%60;
        string minutesConverted = minutes.ToString();
        if (minutesConverted.Length == 1)
            minutesConverted = minutesConverted.Insert(0, "0");
        int hours = (_inGameTime - minutes) / 60;
        string hoursConverted = hours.ToString();
        if (hoursConverted.Length == 1)
            hoursConverted = hoursConverted.Insert(0, "0");
        timeDisplayer.text = hoursConverted + ":" + minutesConverted;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("InGameTime", _inGameTime);
        PlayerPrefs.Save();
    }

    public int GetTime()
    {
        return _inGameTime;
    }
}
