using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private bool multiplayerLevel;
        
        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private GameObject endScreen;
        [SerializeField] private ScoreHandler scoreScript;
        [SerializeField] private Text header;
        [SerializeField] private Text rightCombos;
        [SerializeField] private Text wrongCombos;
        [SerializeField] private Text highestMultiplier;
        [SerializeField] private Text avgTime;
        [SerializeField] private Text score;
        [SerializeField] private Text countDown;
        
        [SerializeField] private Text scoreIndicator;
        [SerializeField] private Text multiplierIndicator;
        [SerializeField] private Text scoreIndicator2;
        [SerializeField] private Text multiplierIndicator2;

        private Animator scoreAnimator;
        private Animator multiplierAnimator;
        private Animator scoreAnimator2;
        private Animator multiplierAnimator2;

        private Animator countDownAnimator;

        private int ListStart;

        private void Awake()
        {
            scoreAnimator = scoreIndicator.GetComponent<Animator>();
            multiplierAnimator = multiplierIndicator.GetComponent<Animator>();

            if (multiplayerLevel)
            {
                scoreAnimator2 = scoreIndicator2.GetComponent<Animator>();
                multiplierAnimator2 = multiplierIndicator2.GetComponent<Animator>();
            }
            
            countDownAnimator = countDown.GetComponent<Animator>();

            pauseScreen.SetActive(false);
            endScreen.SetActive(false);
            countDown.gameObject.SetActive(false);
        }

        public void AnimateScore(string score, int id)
        {
            if (!multiplayerLevel)
            {
                scoreIndicator.text = score;
                scoreAnimator.SetTrigger("ScoreChanged");
            }
            else if (multiplayerLevel)
            {
                if (id == 0)
                {
                    scoreIndicator.text = score;
                    scoreAnimator.SetTrigger("ScoreChanged");
                }
                else if (id == 1)
                {
                    scoreIndicator2.text = score;
                    scoreAnimator2.SetTrigger("ScoreChanged");
                }
            }
        }

        public void AnimateMultiplier(string multiplier, int id)
        {
            if (!multiplayerLevel)
            {
                multiplierIndicator.text = multiplier + " x";
                multiplierAnimator.SetTrigger("MultiplierChanged");
            }
            else if (multiplayerLevel)
            {
                if (id == 0)
                {
                    multiplierIndicator.text = multiplier + " x";
                    multiplierAnimator.SetTrigger("MultiplierChanged");
                }
                else if (id == 1)
                {
                    multiplierIndicator2.text = multiplier + " x";
                    multiplierAnimator2.SetTrigger("MultiplierChanged");
                }
            }
        }

        public void AnimateCountDown(string timeLeft)
        {
            if (!countDown.gameObject.activeSelf)
                countDown.gameObject.SetActive(true);
            countDown.text = timeLeft;
            countDownAnimator.SetTrigger("CountDownChanged");
        }

        public void Pause(bool nowPausing)
        {
            pauseScreen.SetActive(nowPausing);
            if (nowPausing)
                pauseScreen.transform.SetAsLastSibling();
        }

        public void EndScreen(bool won)
        {
            endScreen.SetActive(true);
            endScreen.GetComponent<Animator>().SetTrigger("Ending");
            endScreen.transform.SetAsLastSibling();

            if (!multiplayerLevel)
            {
                header.text = won ? "Level Passed" : "Level Failed";
                rightCombos.text = scoreScript.CorrectPoints.ToString();
                wrongCombos.text = "- " + scoreScript.WrongPenalty.ToString();
                highestMultiplier.text = scoreScript.HighestMultiplier.ToString();
                string tempTime = scoreScript.AverageTime().ToString();
                if (tempTime != "NAN") 
                    avgTime.text = scoreScript.AverageTime().ToString("F2");
                scoreScript.CalcEndScore();
                score.text = scoreScript.TotalScore.ToString();
            }
            else
            {
                int scoreP1 = GameObject.FindWithTag("scorehandler").GetComponent<ScoreHandler>().TotalScore;
                int scoreP2 = GameObject.FindWithTag("scorehandler2").GetComponent<ScoreHandler>().TotalScore;

                if (scoreP1 == scoreP2)
                {
                    header.text = "It's a tie!";
                    rightCombos.text = scoreP1.ToString();
                    wrongCombos.text = scoreP2.ToString();
                }
                else if (scoreP1 > scoreP2)
                {
                    header.text = "Player 1 wins!";
                    rightCombos.text = scoreP1.ToString();
                    wrongCombos.text = scoreP2.ToString();
                }
                else if (scoreP2 > scoreP1)
                {
                    header.text = "Player 2 wins!";
                    rightCombos.text = scoreP2.ToString();
                    wrongCombos.text = scoreP1.ToString();
                }
            }
        }
    }
}