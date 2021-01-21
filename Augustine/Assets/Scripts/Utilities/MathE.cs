using UnityEngine;

namespace Utilities
{
  public class MathE
  {
    /// <summary>
    /// Rounds the float up if >= 0.5f otherwise rounds down
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static int RoundToInt(float f)
    {
      return RoundToInt(f, 0.5f);
    }

    /// <summary>
    /// Rounds the float up if >= upperThreshold otherwise rounds down
    /// </summary>
    /// <param name="f"></param>
    /// <param name="upperThreshold"></param>
    /// <returns></returns>
    public static int RoundToInt(float f, float upperThreshold)
    {
      upperThreshold = Mathf.Clamp(f, 0f, 1f);

      if (f % 1f >= upperThreshold) return Mathf.CeilToInt(f); else return Mathf.FloorToInt(f);
    }
  }
}