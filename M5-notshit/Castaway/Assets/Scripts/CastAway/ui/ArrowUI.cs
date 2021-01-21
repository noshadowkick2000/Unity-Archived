using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using gameObjects;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
    public class ArrowUI : MonoBehaviour
    {
        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject upArrow;
        [SerializeField] private GameObject rightArrow;
        [SerializeField] private GameObject downArrow;
        [SerializeField] private float arrowSizeDivider;

        [SerializeField] private float yOffsetZombie;
        private float prefabWidth;

        private int ListStart;

        private bool levelOver = false;

        //holds order of prefab images
        private int amtOfPlayers;
        private List<GameObject>[] uiCombo;
        //private List<GameObject> uiCombo2 = new List<GameObject>();
        
        private void Awake()
        {
            StartCoroutine(AfterStart());
        }

        private void Start()
        {
            amtOfPlayers = FindObjectsOfType<Player>().Length;
            uiCombo = new List<GameObject>[amtOfPlayers];
            for (int i =0; i<uiCombo.Length; i++)
                uiCombo[i] = new List<GameObject>();
        }

        private IEnumerator AfterStart()
        {
            yield return 0;
            prefabWidth = GetComponent<RectTransform>().rect.width / arrowSizeDivider;
            leftArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(prefabWidth, prefabWidth);
            upArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(prefabWidth, prefabWidth);
            rightArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(prefabWidth, prefabWidth);
            downArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(prefabWidth, prefabWidth);
        }

        public void CreateZombieUI(KeyCode[] elements, int comboindex, int playerID)
        {
            if (!levelOver)
            {
                ListStart = comboindex;

                for (int i = ListStart; i < elements.Length; i++)
                {
                    switch (elements[i])
                    {
                        case KeyCode.LeftArrow:
                            GameObject templeft = Instantiate(leftArrow, transform);
                            //if (playerID==0)
                                uiCombo[playerID].Add(templeft);
                            //else if (playerID==1)
                                //uiCombo2.Add(templeft);
                            break;
                        case KeyCode.UpArrow:
                            GameObject tempup = Instantiate(upArrow, transform);
                            //if (playerID==0)
                                uiCombo[playerID].Add(tempup);
                            //else if (playerID==1)
                                //uiCombo2.Add(tempup);
                            break;
                        case KeyCode.RightArrow:
                            GameObject tempright = Instantiate(rightArrow, transform);
                            //if (playerID==0)
                                uiCombo[playerID].Add(tempright);
                            //else if (playerID==1)
                                //uiCombo2.Add(tempright);
                            break;
                        case KeyCode.DownArrow:
                            GameObject tempdown = Instantiate(downArrow, transform);
                            //if (playerID==0)
                                uiCombo[playerID].Add(tempdown);
                            //else if (playerID==1)
                                //uiCombo2.Add(tempdown);
                            break;
                    }
                }
            }
        }

        public void MoveComboUI(Vector3 zombiePos, int playerID)
        {
            Vector3 zombiePosOnScreen = Camera.main.WorldToScreenPoint(zombiePos);
            Vector3 center = zombiePosOnScreen + new Vector3(0, yOffsetZombie, 0);
            float startingX = 0;
                
            //if (playerID == 0)
            //{
                //if even/odd
                if (uiCombo[playerID].Count % 2 == 0)
                {
                    startingX = -((prefabWidth * ((uiCombo[playerID].Count / 2) - 0.5f)));
                }
                else if (uiCombo[playerID].Count == 1)
                {
                    startingX = 0;
                }
                else
                {
                    startingX = -((((uiCombo[playerID].Count - 1) / 2) * prefabWidth));
                }

                Vector3 startingPointSpawn = center + new Vector3(startingX, 0, 0);
                startingPointSpawn.z = 0;

                for (int i = 0; i < uiCombo[playerID].Count; i++)
                {
                    uiCombo[playerID][i].SetActive(true);
                    uiCombo[playerID][i].GetComponent<RectTransform>().position = (startingPointSpawn + new Vector3(i * prefabWidth, 0, 0));
                }
            //}
            /*else if (playerID==1)
            {
                //if even/odd
                if (uiCombo2.Count % 2 == 0)
                {
                    startingX = -((prefabWidth * ((uiCombo2.Count / 2) - 0.5f)));
                }
                else if (uiCombo2.Count == 1)
                {
                    startingX = 0;
                }
                else
                {
                    startingX = -((((uiCombo2.Count - 1) / 2) * prefabWidth));
                }

                Vector3 startingPointSpawn = center + new Vector3(startingX, 0, 0);
                startingPointSpawn.z = 0;

                for (int i = 0; i < uiCombo2.Count; i++)
                {
                    uiCombo2[i].SetActive(true);
                    uiCombo2[i].GetComponent<RectTransform>().position = (
                        startingPointSpawn + new Vector3(i * prefabWidth, 0, 0));
                }   
            }*/
        }

        public void DestroyComboUI(int playerID)
        {
            //if (playerID == 0)
            //{
                for (int i = 0; i < uiCombo[playerID].Count; i++)
                {
                    GameObject temp = uiCombo[playerID][i];
                    Destroy(temp);
                }

                uiCombo[playerID].Clear();
            //}
            /*else if (playerID == 1)
            {
                for (int i = 0; i < uiCombo2.Count; i++)
                {
                    GameObject temp = uiCombo2[i];
                    Destroy(temp);
                }

                uiCombo2.Clear();
            }*/
        }

        public void RemoveSingleCombo(int playerID)
        {
            //if (playerID == 0)
            //{
                if (uiCombo[playerID].Count > 0)
                {
                    GameObject temp = uiCombo[playerID][0];
                    Destroy(temp);
                    uiCombo[playerID].Remove(temp);
                }
            //}
            /*else if (playerID == 1)
            {
                if (uiCombo2.Count > 0)
                {
                    GameObject temp = uiCombo2[0];
                    Destroy(temp);
                    uiCombo2.Remove(temp);
                }
            }*/
        }
    }
}