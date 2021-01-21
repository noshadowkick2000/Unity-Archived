using System;
using System.Collections;
using gameCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace menuHub
{
    public class DecorManager : MonoBehaviour
    {
        private int currentProgress;

        //private GameCore _gameCore;

        private GameObject _boat1;
        private GameObject _boat2;
        private GameObject _boat3;
        private GameObject _boat4;
        private GameObject _fire;

        public bool isNight;

        private float waitTime = 1.5f;

        void Awake()
        {
            //_gameCore = GameObject.FindGameObjectWithTag("gamecore").GetComponent<GameCore>();
            _fire = GameObject.FindGameObjectWithTag("firelight");

            currentProgress = GetComponent<GameCore>().progress;

            setScenery();
        }

        private void setScenery()
        {
            _boat1 = GameObject.FindGameObjectWithTag("day1");
            _boat2 = GameObject.FindGameObjectWithTag("day2");
            _boat3 = GameObject.FindGameObjectWithTag("day3");
            _boat4 = GameObject.FindGameObjectWithTag("day4");

            switch (currentProgress)
            {
                case 0:
                    _boat1.SetActive(true);
                    _boat2.SetActive(false);
                    _boat3.SetActive(false);
                    _boat4.SetActive(false);
                    break;
                case 1:
                    _boat1.SetActive(false);
                    _boat2.SetActive(true);
                    _boat3.SetActive(false);
                    _boat4.SetActive(false);
                    break;
                case 2:
                    _boat1.SetActive(false);
                    _boat2.SetActive(false);
                    _boat3.SetActive(true);
                    _boat4.SetActive(false);
                    break;
                case 3:
                    _boat1.SetActive(false);
                    _boat2.SetActive(false);
                    _boat3.SetActive(false);
                    _boat4.SetActive(true);
                    break;
            }

            int curSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            if (curSceneBuildIndex == 1 || curSceneBuildIndex == 2)
                isNight = true;
            else
                isNight = false;


            if (!isNight)
            {
                StartCoroutine(turnFireLight(false));
            }
            else
            {
                _fire.SetActive(false);
                StartCoroutine(turnFireLight(true));
            }
        }

        private IEnumerator turnFireLight(bool on)
        {
            if (on)
            { 
                yield return new WaitForSecondsRealtime(waitTime);
                _fire.SetActive(true);
            }
            else
            {
                yield return new WaitForSecondsRealtime(waitTime*2);
                _fire.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                PlayerPrefs.DeleteAll();
        }
    }
}
