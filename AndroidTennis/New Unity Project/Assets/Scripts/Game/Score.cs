using System.Collections;
using System.Collections.Generic;
using Game;
//using UnityEditor.Experimental.GraphView; <- caused error in build
using UnityEngine;

public class Score : Singleton<Score>
{
    private int _playerOneScore;
    private int _playerTwoScore;

    private int _playerOneTimesKnockedDown;
    private int _playerOneRacketsUsed;
    
    private int _playerTwoTimesKnockedDown;
    private int _playerTwoRacketsUsed;

    private string IntScoreToTennisScore(int points)
    {
        if (points == 0)
        {
            return "love";
        }
        else if (points == 1)
        {
            return "15";
        }
        else
        {
            return ((points * 10) + 10).ToString();
        }
    }

    public string ReturnScore(bool playerOne)
    {
        return IntScoreToTennisScore(playerOne ? _playerOneScore : _playerTwoScore);
    }

    public int ReturnRacketsUsed(bool playerOne)
    {
        return playerOne ? _playerOneRacketsUsed : _playerTwoRacketsUsed;
    }

    public void IncrementRackets(bool playerOne)
    {
        if (playerOne)
            _playerOneRacketsUsed++;
        else
            _playerTwoRacketsUsed++;
    }

    public int ReturnTimesKnockedDown(bool playerOne)
    {
        return playerOne ? _playerOneTimesKnockedDown : _playerTwoTimesKnockedDown;
    }
    
    public void IncrementKnockDown(bool playerOne)
    {
        if (playerOne)
            _playerOneTimesKnockedDown++;
        else
            _playerTwoTimesKnockedDown++;
    }

    // Return whether game is set
    public bool GivePoint(bool playerOne)
    {
        if (playerOne)
        {
            _playerOneScore++;
        }
        else
        {
            _playerTwoScore++;
        }

        if (Game())
        {
            // Someone won
            print("Set");
            return true;
        }
        else
        {
            // Continue game
            return false;
        }
    }

    private bool Game()
    {
        int highestScore;
        int lowestScore;
        if (_playerOneScore > _playerTwoScore)
        {
            highestScore = _playerOneScore;
            lowestScore = _playerTwoScore;
        }
        else
        {
            lowestScore = _playerOneScore;
            highestScore = _playerTwoScore;
        }

        return (highestScore > 3 && highestScore-lowestScore > 1);
    }

    public void ResetScore()
    {
        _playerOneScore = 0;
        _playerTwoScore = 0;
    }

    public void SaveScore()
    {
        int totalScore = (_playerOneScore - _playerTwoScore) * (_playerOneRacketsUsed + _playerOneTimesKnockedDown);
        SaveManager.AddScore(totalScore);
    }
}
