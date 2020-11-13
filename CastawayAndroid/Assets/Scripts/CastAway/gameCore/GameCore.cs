using UnityEngine;
using UnityEngine.SceneManagement;

namespace gameCore
{
    public class GameCore : MonoBehaviour
    {
        //progress is PlayerPrefs LevelProgress, saves which day is the current day
        public int progress;
        [SerializeField] private int overrideProgress;

        private void Awake()
        {
            if (overrideProgress == 0)
            {
                progress = PlayerPrefs.GetInt("LevelProgress");
            }
            else
                progress = overrideProgress;
        }

        public void ChangeScene(int sceneBuildIndex)
        {
            PlayerPrefs.Save();
            //0: menu, 1: fight, 2: fightmulti, 3: outro
            SceneManager.LoadScene(sceneBuildIndex);
        }

        public void ProgressToNextLevel()
        {
            PlayerPrefs.SetInt("LevelProgress", progress+1);
        }

        public void ResetProgress()
        {
            PlayerPrefs.SetInt("LevelProgress", 1);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("startup", 0);
            PlayerPrefs.Save();
        }
    }
}