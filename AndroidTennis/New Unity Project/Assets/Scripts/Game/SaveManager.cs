using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game
{
  public static class SaveManager
  {
    private static string _saveLocation = Application.persistentDataPath + "/CTSave.data";

    public static void DeleteSave()
    {
      if (File.Exists(_saveLocation))
        File.Delete(_saveLocation);
    }
    
    public static void AddScore(int newScore)
    {
      FileStream fileStream;
      List<int> readScore = new List<int>();
      
      if (!File.Exists(_saveLocation))
      {
        fileStream = File.Create(_saveLocation);
        readScore.Append(newScore);
      }
      else
      {
        readScore = ReadScore();
        fileStream = File.OpenWrite(_saveLocation);
      }

      BinaryFormatter binaryFormatter = new BinaryFormatter();
      binaryFormatter.Serialize(fileStream, readScore);
      fileStream.Close();
    }

    public static List<int> ReadScore()
    {
      FileStream fileStream = File.OpenRead(_saveLocation);
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      List<int> scores = (List<int>) binaryFormatter.Deserialize(fileStream);
      fileStream.Close();
      return scores;
    }
  }

  /*public class ScoreSave
  {
    public int[] highscores;

    public ScoreSave(int[] savedScores)
    {
      highscores = savedScores;
    }
  }*/
}