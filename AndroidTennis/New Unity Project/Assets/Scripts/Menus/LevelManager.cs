using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Settings")]
    [Tooltip("Enter the name of the scene to be loaded on startup")] public string sceneToLoad;
    public bool loadOnStart = false;
    [Header("Transition Settings")]
    public Animator transition;
    public float transitionTime = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        //start async operation
        if (loadOnStart)
        {
            StartCoroutine(LoadScene(sceneToLoad));
            Debug.Log("Loading Scene: " + sceneToLoad);
        }
    }

    public virtual void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
        Debug.Log("Loading Scene: " + sceneName);
    }

    public virtual void Exit()
    {
        StartCoroutine(ExitSequence());
        Debug.Log("Exiting Application..");
    }

    IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        //create async operation
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator ExitSequence()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        Application.Quit();
    }
}
