using UnityEngine;

namespace DelaunayGeneration
{
  public class Line2D
  {
    public Vector2 P0 { get; }
    public Vector2 P1 { get; }
    public float Radius { get; }

    public Line2D(Vector2 p0, Vector2 p1, float radius)
    {
      P0 = p0;
      P1 = p1;
      Radius = radius;
    }
  }
}