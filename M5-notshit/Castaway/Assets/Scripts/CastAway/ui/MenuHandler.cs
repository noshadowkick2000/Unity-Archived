using System.Collections;
using System.Collections.Generic;
using System.Linq;
using gameCore;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
    public class MenuHandler : MonoBehaviour
    {
        [SerializeField] private GameObject MenuButtons;
        [SerializeField] private GameObject _creditsPanel;
        [SerializeField] private GameObject _leaderboardPanel;
        [SerializeField] private GameObject _controlsPanel;
        [SerializeField] private Text hiscore1;
        [SerializeField] private Text hiscore2;
        [SerializeField] private Text hiscore3;
        [SerializeField] private GameObject fader;
        [SerializeField] private GameObject[] StartObjects;

        // Start is called before the first frame update
        void Start()
        {
            _creditsPanel.SetActive(false);
            _leaderboardPanel.SetActive(false);
            _controlsPanel.SetActive(false);
            if (PlayerPrefs.GetInt("startup")==0)
            {
                fader.SetActive(false);
                HideAll();
                PlayerPrefs.SetInt("startup", 1);
                PlayerPrefs.Save();
                Camera.main.GetComponent<Animator>().SetBool("inMenu", false);
            }
            else
            {
                for (int i = 0; i < StartObjects.Length; i++)
                {
                    StartObjects[i].SetActive(false);
                }
                Camera.main.GetComponent<Animator>().SetBool("inMenu", true);
            }
        }

        public void StartButton()
        {
            ResetAll();
            for (int i = 0; i < StartObjects.Length; i++)
            {
                StartObjects[i].SetActive(false);
            }
            Camera.main.GetComponent<Animator>().SetBool("inMenu", true);
        }

        private IEnumerator WaitForAnim(bool multiplayer)
        {
            Camera.main.GetComponent<Animator>().SetBool("inMenu", false);
            yield return new WaitForSecondsRealtime(2);
            GameCore temp = GameObject.FindGameObjectWithTag("gamecore").GetComponent<GameCore>();
            if (!multiplayer)
            {
                temp.ChangeScene(1);
            }
            else
            {
                temp.ChangeScene(2);
            }
        }

        private void HideAll()
        {
            MenuButtons.SetActive(false);
        }

        private void ResetAll()
        {
            MenuButtons.SetActive(true);
        }

        public void ShowCredits()
        {
            if (!_creditsPanel.activeSelf)
            {
                HideAll();
                _creditsPanel.SetActive(true);
                _creditsPanel.GetComponent<Animator>().SetTrigger("Ending");
            }
        }

        public void HideCredits()
        {
            if (_creditsPanel.activeSelf)
            {
                _creditsPanel.SetActive(false);
                ResetAll();
            }
        }

        public void ShowControls()
        {
            if (!_controlsPanel.activeSelf)
            {
                HideAll();
                _controlsPanel.SetActive(true);
                _controlsPanel.GetComponent<Animator>().SetTrigger("Ending");
            }
        }

        public void HideControls()
        {
            if (_controlsPanel.activeSelf)
            {
                _controlsPanel.SetActive(false);
                ResetAll();
            }
        }

        public void ShowLeaderBoard()
        {
            if (!_leaderboardPanel.activeSelf)
            {
                HideAll();
                _leaderboardPanel.SetActive(true);
                _leaderboardPanel.GetComponent<Animator>().SetTrigger("Ending");
                hiscore1.text = PlayerPrefs.GetInt("score1").ToString();
                hiscore2.text = PlayerPrefs.GetInt("score2").ToString();
                hiscore3.text = PlayerPrefs.GetInt("score3").ToString();
            }
        }

        public void HideLeaderBoard()
        {
            if (_leaderboardPanel.activeSelf)
            {
                _leaderboardPanel.SetActive(false);
                ResetAll();
            }
        }


        public void StartLevel()
        {
            if (GameObject.FindWithTag("gamecore").GetComponent<GameCore>().progress < 7) //prob unnecasary
            {
                MenuButtons.GetComponent<Animator>().SetTrigger("Ending");
                StartCoroutine(WaitForAnim(false));
            }
        }

        public void StartMultiplayerLevel()
        {
            MenuButtons.GetComponent<Animator>().SetTrigger("Ending");
            StartCoroutine(WaitForAnim(true));
        }

        public void QuitApplication()
        {
            PlayerPrefs.SetInt("startup", 0);
            Application.Quit();
        }
    }
}