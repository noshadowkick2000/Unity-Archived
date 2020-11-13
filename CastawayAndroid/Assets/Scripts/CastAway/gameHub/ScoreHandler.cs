using FMODUnity;
using gameCore;
using ui;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    private int score;
    private int comboMultiplier;

    [SerializeField] private int id;

    [SerializeField] private int pointsPerCorrectCombo;
    [SerializeField] private int pointsPerCorrectZombie;
    [SerializeField] private int pointsPerCorrectComboSpecial;
    [SerializeField] private int pointsPerCorrectZombieSpecial;
    [SerializeField] private int pointsPerCorrectBigZombie;
    [SerializeField] private int multiplierUpAmount;
    [SerializeField] private int penaltyWrongCombo;
    [SerializeField] private UI targetUI;
    [SerializeField] private StudioEventEmitter uiSound;

    public int CorrectPoints;
    public int CorrectZombiePoints;
    public int WrongPenalty;
    public int Multiplier;
    public int HighestMultiplier;
    private float AvgZombieTime;
    private float TotalZombieTime;
    private int AmountOfZombiesInLevel;
    public int TotalScore;

    private void Start()
    {
        CorrectPoints = 0;
        CorrectZombiePoints = 0;
        WrongPenalty = 0;
        Multiplier = 1;
        HighestMultiplier = 0;
        AvgZombieTime = 0;
        AmountOfZombiesInLevel = 0;
        TotalScore = 0;
    }

    private void CalcTotalScore()
    {
        TotalScore = CorrectPoints + CorrectZombiePoints - WrongPenalty;
    }

    public void AddComboPoints(bool isSpecialZombie)
    {
        if (isSpecialZombie)
            CorrectPoints += pointsPerCorrectComboSpecial * Multiplier;
        else
            CorrectPoints += pointsPerCorrectCombo * Multiplier;
        
        CalcTotalScore();
        targetUI.AnimateScore(TotalScore.ToString(), id);
        uiSound.SetParameter("CorrectCombo", 1);
        uiSound.SetParameter("CorrectCombo", 0);
    }

    public void SubtractComboPoints()
    {
        WrongPenalty += penaltyWrongCombo;
        Multiplier = 0;
        CalcTotalScore();
        targetUI.AnimateMultiplier(Multiplier.ToString(), id);
        targetUI.AnimateScore(TotalScore.ToString(), id);
        
        uiSound.SetParameter("WrongCombo", 1);
        uiSound.SetParameter("WrongCombo", 0);
    }

    public void AddZombieComboPoints(bool isSpecialZombie, float completionTime)
    {
        if (Multiplier<=6)
            uiSound.SetParameter("MultiplierAmount", Multiplier-1);
        uiSound.SetParameter("CorrectZombie", 1);
        uiSound.SetParameter("CorrectZombie", 0);
        
        if (isSpecialZombie)
            CorrectZombiePoints += pointsPerCorrectZombieSpecial * Multiplier;
        else
            CorrectZombiePoints += pointsPerCorrectZombie * Multiplier;

        Multiplier += multiplierUpAmount;
        if (Multiplier > HighestMultiplier)
            HighestMultiplier = Multiplier;
        
        CalcTotalScore();

        TotalZombieTime += completionTime;
        AmountOfZombiesInLevel += 1;
        
        targetUI.AnimateMultiplier(Multiplier.ToString(), id);
        targetUI.AnimateScore(TotalScore.ToString(), id);
    }

    public void AddBigZombieComboPoints()
    {
        CorrectPoints += pointsPerCorrectBigZombie;
        
        CalcTotalScore();
        
        targetUI.AnimateScore(TotalScore.ToString(), id);
        uiSound.SetParameter("CorrectZombie", 1);
        uiSound.SetParameter("CorrectZombie", 0);
    }

    //only call at the END of the level
    public float AverageTime()
    {
        AvgZombieTime = TotalZombieTime / AmountOfZombiesInLevel;
        return AvgZombieTime;
    }

    //only called at end of level
    public void CalcEndScore()
    {
        CalcTotalScore();
        float timeMultiplier = 2 - (AvgZombieTime/3);
        TotalScore = Mathf.RoundToInt(TotalScore * timeMultiplier);

        int curLevel = GameObject.FindWithTag("gamecore").GetComponent<GameCore>().progress;

        switch (curLevel)
        {
            case 0:
                if (PlayerPrefs.GetInt("score1") < TotalScore)
                {
                    PlayerPrefs.SetInt("score1", TotalScore);
                    PlayerPrefs.Save();
                }
                break;
            case 1:
                if (PlayerPrefs.GetInt("score2") < TotalScore)
                {
                    PlayerPrefs.SetInt("score2", TotalScore);
                    PlayerPrefs.Save();
                }
                break;
            case 2:
                if (PlayerPrefs.GetInt("score3") < TotalScore)
                {
                    PlayerPrefs.SetInt("score3", TotalScore);
                    PlayerPrefs.Save();
                }
                break;
        }
    }
}
