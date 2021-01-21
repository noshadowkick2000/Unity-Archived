using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DelaunayGeneration
{
  public class HallwayConnection
  {
    public bool IsLShaped { get; }

    public Line2D Line0 { get; }
    // Only used if is L Shaped
    public Line2D Line1 { get; }
    public int P0 { get; }
    public int P1 { get; }
    public List<int> RoomsOnConnection = new List<int>();

    public HallwayConnection(Vector2 line0Point0, Vector2 line0Point1, float radius, int p0, int p1)
    {
      IsLShaped = false;

      Line0 = new Line2D(line0Point0, line0Point1, radius);
      P0 = p0;
      P1 = p1;

      Line1 = null;
    }

    public HallwayConnection(Vector2 line0Point0, Vector2 line0Point1, float radius0, Vector2 line1Point0, Vector2 line1Point1, float radius1, int p0, int p1)
    {
      IsLShaped = true;

      Line0 = new Line2D(line0Point0, line0Point1, radius0);
      P0 = p0;
      P1 = p1;
      Line1 = new Line2D(line1Point0, line1Point1, radius1);
    }

    public void AddRoomsOnConnection(IEnumerable<int> roomIndices)
    {
      RoomsOnConnection.Union(roomIndices);
    }

    public void SortRooms()
    {

    }
  }
}