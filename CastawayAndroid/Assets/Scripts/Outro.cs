using System;
using System.Collections;
using FMODUnity;
using gameCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Outro : MonoBehaviour
{

    [SerializeField] private StudioEventEmitter outro;

    private void Start()
    {
        StartCoroutine(WaitMusic());
        StartCoroutine(WaitScene());
    }
    
    private IEnumerator WaitMusic()
    {
        yield return new WaitForSecondsRealtime(5);
        outro.SetParameter("State", 1);
    }

    private IEnumerator WaitScene()
    {
        yield return new WaitForSecondsRealtime(10);
        ToStartScreen();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ToStartScreen();
        }
    }

    public void ToStartScreen()
    {
        GameObject.FindWithTag("gamecore").GetComponent<GameCore>().ChangeScene(0);
    }
}
