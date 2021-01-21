using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreStuff
{
    public class Test : MonoBehaviour
    {
        private static Test Core;

        //[SerializeField] private int _startScene;

        private int _sceneIndex;

        private void Awake()
        {
            _sceneIndex = SceneManager.GetActiveScene().buildIndex;
        }

        private void Start()
        {
            if (Core != null)
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                GameObject.DontDestroyOnLoad(gameObject);
                Core = this;
            }
        }

        public void ChangeScene(bool _continue)
        {
            if (_continue)
            {
                
                if (_sceneIndex < SceneManager.sceneCountInBuildSettings - 1)
                {                
                    _sceneIndex += 1;
                    SceneManager.LoadScene(_sceneIndex);
                }
            }
            else
            {
                if (_sceneIndex > 0) 
                {
                    _sceneIndex -= 1;
                    SceneManager.LoadScene(_sceneIndex);
                }
            }
        }
    }
}