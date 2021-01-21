using System.Collections;
using System.Collections.Generic;

using TriangleNet.Geometry;

using UnityEngine;

namespace DelaunayGeneration
{
  [System.Flags] public enum GridSpace
  {
    // Unoccupied space
    Empty = 0,
    // Physical space
    Wall = 1,
    Floor = 2,
    // Physical property
    Hub = 8,
    Hallway = 16,
    Corridor = 32,
  }

  public class DungeonNodesData
  {
    // Holds the key as the room id, and DungeonRoom which contains the position and scale of the room
    public Dictionary<int, DungeonRoom> DungeonRooms = new Dictionary<int, DungeonRoom>();
    // Holds the key as the room id of the vertice, and the Vertex which contains the position of the vertice
    public Dictionary<int, Vector2> Vertices = new Dictionary<int, Vector2>();
    // Holds the key as the room id, and the value as a list of room ids that it connects to
    private Dictionary<int, List<int>> AdjacencyList = new Dictionary<int, List<int>>();
    private List<Edge> m_edges = new List<Edge>();

    private List<HallwayConnection> m_hallwayConnections = new List<HallwayConnection>();
    private GridSpace[,] m_dungeonGridSpace;

    private Vector2Int m_worldToGrid = new Vector2Int();

    public IEnumerable<Edge> Edges { get { return m_edges; } }
    public IEnumerable<HallwayConnection> HallwayConnections { get { return m_hallwayConnections; } }

    public void AddRoom(int id, GameObject roomCube, RoomType roomType)
    {
      Vector2 roomPosition = new Vector2(roomCube.transform.position.x, roomCube.transform.position.z);
      Vector2 roomScale = new Vector2(roomCube.transform.localScale.x, roomCube.transform.localScale.z);

      DungeonRoom room = new DungeonRoom(id, roomPosition, roomScale, roomType);
      DungeonRooms.Add(id, room);
    }
    
    public void AddEdge(Vector2 v0, int v0id, Vector2 v1, int v1id)
    {
      if (!Vertices.ContainsKey(v0id)) Vertices.Add(v0id, v0);
      if (!Vertices.ContainsKey(v1id)) Vertices.Add(v1id, v1);

      Edge edge = new Edge(v0id, v1id);
      m_edges.Add(edge);

      if (!AdjacencyList.ContainsKey(v0id))
      {
        AdjacencyList.Add(v0id, new List<int>());
      }      
      
      if (!AdjacencyList.ContainsKey(v1id))
      {
        AdjacencyList.Add(v1id, new List<int>());
      }

      AdjacencyList[v0id].Add(v1id);
      AdjacencyList[v1id].Add(v0id);
    }

    public void CreateHallwayLines()
    {
      if (m_hallwayConnections.Count != 0) return;

      foreach (Edge edge in m_edges)
      {
        int index0 = edge.P0;
        int index1 = edge.P1;

        // Get position of rooms
        Vector2 r0Pos = DungeonRooms[index0].Position;
        Vector2 r1Pos = DungeonRooms[index1].Position;

        // Get y and x diffs
        float yDiff = Mathf.Max(r0Pos.y, r1Pos.y) - Mathf.Min(r0Pos.y, r1Pos.y);
        float xDiff = Mathf.Max(r0Pos.x, r1Pos.x) - Mathf.Min(r0Pos.x, r1Pos.x);

        // Find if the edge is horizontal or vertical
        float degrees = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;

        // If < 45 degrees it will be horizontal, else vertical
        bool horizontalEdge = degrees < 45f;

        // Get scale of rooms
        Vector2 r0Scale = DungeonRooms[index0].Scale;
        Vector2 r1Scale = DungeonRooms[index1].Scale;

        // Midpoint of v0 and v1, y for horizontal connection, x for vertical connection
        Vector2 midpoint = (r0Pos + r1Pos) / 2f;
        float midpointToCheck = horizontalEdge ? midpoint.y : midpoint.x;

        // The boundaries of the room on the same axis as the midpointToCheck
        float room0upperBoundary = horizontalEdge ? r0Pos.y + r0Scale.y / 2f: r0Pos.x + r0Scale.x / 2f;
        float room0lowerBoundary = horizontalEdge ? r0Pos.y - r0Scale.y / 2f: r0Pos.x - r0Scale.x / 2f;
        float room1upperBoundary = horizontalEdge ? r1Pos.y + r1Scale.y / 2f : r1Pos.x + r1Scale.x / 2f;
        float room1lowerBoundary = horizontalEdge ? r1Pos.y - r1Scale.y / 2f : r1Pos.x - r1Scale.x / 2f;

        bool midpointIsWithinBoundaries = midpointToCheck <= room0upperBoundary && midpointToCheck >= room0lowerBoundary &&
          midpointToCheck <= room1upperBoundary && midpointToCheck >= room1lowerBoundary;

        // Check if midpoint is within the two boundaries
        if (midpointIsWithinBoundaries)
        {
          // Can form a straight connection, only need 2 points
          Vector2 firstPoint, secondPoint;

          if (horizontalEdge)
          {
            firstPoint = new Vector2(r0Pos.x, midpointToCheck);
            secondPoint = new Vector2(r1Pos.x, midpointToCheck);
          }

          else
          {
            firstPoint = new Vector2(midpointToCheck, r0Pos.y);
            secondPoint = new Vector2(midpointToCheck, r1Pos.y);
          }

          float radius = (midpointToCheck % 1f == 0f) ? 2f : 2.5f;

          m_hallwayConnections.Add(new HallwayConnection(firstPoint, secondPoint, radius, index0, index1));
        }

        // L Shaped connection
        else
        {
          // Randomise which axis to form the first connection
          bool startHorizontally = Random.value > 0.5f;

          Vector2 firstPoint = r0Pos, secondPoint = r0Pos, thirdPoint = r1Pos;

          if (startHorizontally)
          {
            secondPoint.x = r1Pos.x;
          }

          else
          {
            secondPoint.y = r1Pos.y;
          }

          float firstRadius = (startHorizontally && (firstPoint.y % 1f == 0f)) || (!startHorizontally && (firstPoint.x % 1f == 0f)) ? 2f : 2.5f;
          float secondRadius = (startHorizontally && (thirdPoint.x % 1f == 0f)) || (!startHorizontally && (thirdPoint.y % 1f == 0f)) ? 2f : 2.5f;

          m_hallwayConnections.Add(new HallwayConnection(firstPoint, secondPoint, firstRadius, secondPoint, thirdPoint, secondRadius, index0, index1));
        }
      }
    }

    // Should be moved into DungeonNodesData
    public IEnumerator AddHallwayRooms(GameObject[] roomCubes)
    {
      yield return new WaitForFixedUpdate();

      foreach (HallwayConnection connection in HallwayConnections)
      {
        var roomsOnConnection = SpherecastHallwayRooms(connection.Line0.P0, connection.Line0.P1, connection.Line0.Radius, roomCubes);
        connection.AddRoomsOnConnection(roomsOnConnection);

        if (connection.IsLShaped)
        {
          roomsOnConnection = SpherecastHallwayRooms(connection.Line1.P0, connection.Line1.P1, connection.Line1.Radius, roomCubes);
          connection.AddRoomsOnConnection(roomsOnConnection);
        }

        // Sort the rooms on connection ids

      }

      foreach (GameObject roomCube in roomCubes)
      {
        Object.Destroy(roomCube);
      }

      Generate2DGrid();
    }

    // Should be moved into DungeonNodesData
    private IEnumerable<int> SpherecastHallwayRooms(Vector2 start, Vector2 end, float radius, GameObject[] roomCubes)
    {
      Vector3 origin = new Vector3(start.x, 0f, start.y);
      Vector3 direction = end - start;
      direction.z = direction.y;
      direction.y = 0f;

      float maxDistance = direction.magnitude;

      Ray ray = new Ray(origin, direction);

      // Spherecast and find objects that intersect with the hallway
      // Increase radius by a tiny bit since we want to collide with rooms that will be just touching the hallway
      RaycastHit[] hitInfo = Physics.SphereCastAll(ray, radius + 0.1f, maxDistance);

      if (hitInfo.Length > 0)
      {
        List<int> indicesOfRooms = new List<int>();

        foreach (RaycastHit hit in hitInfo)
        {
          // Get the id of the room
          int index = System.Array.IndexOf(roomCubes, hit.collider.gameObject);
          indicesOfRooms.Add(index);

          if (!DungeonRooms.ContainsKey(index))
          {
            // DEBUG
            hit.collider.GetComponent<MeshRenderer>().material.color = Color.blue;

            AddRoom(index, hit.collider.gameObject, RoomType.Hallway);
          }
        }

        return indicesOfRooms;
      }

      else return null;
    }

    public void Generate2DGrid()
    {
      // The grid indices take from the top left corner of the square, so a grid index at [1][1] will occupy the square at 1.5, 0.5

      int topMost = int.MinValue, bottomMost = int.MaxValue, leftMost = int.MaxValue, rightMost = int.MinValue;

      // Find the dimensions of the dungeon by finding the most extreme axis of every room
      foreach (DungeonRoom room in DungeonRooms.Values)
      {
        int top = Mathf.RoundToInt(room.Position.y + room.Scale.y / 2f);
        int bottom = Mathf.RoundToInt(room.Position.y - room.Scale.y / 2f);
        int left = Mathf.RoundToInt(room.Position.x - room.Scale.x / 2f);
        int right = Mathf.RoundToInt(room.Position.x + room.Scale.x / 2f);

        if (top > topMost) topMost = top;
        if (bottom < bottomMost) bottomMost = bottom;
        if (left < leftMost) leftMost = left;
        if (right > rightMost) rightMost = right;
      }

      // Because of the way the grid is calculated, the whole grid space has to be moved up one grid
      ++bottomMost;
      ++topMost;

      int xDimension = Mathf.Abs(rightMost - leftMost);
      int yDimension = Mathf.Abs(topMost - bottomMost);

      Debug.Log("Dungeon dimension: " + xDimension + ", " + yDimension);

      // The grid space will have only positive indices, all the coordinates will be normalised into positive space
      m_dungeonGridSpace = new GridSpace[xDimension, yDimension];

      // First, fill out every space as empty
      for (int x = 0; x < xDimension; ++x)
      {
        for (int y = 0; y < yDimension; ++y)
        {
          m_dungeonGridSpace[x, y] = GridSpace.Empty;
        }
      }

      m_worldToGrid.x = Mathf.Abs(leftMost);
      m_worldToGrid.y = Mathf.Abs(bottomMost);

      // Secondly, generate the rooms in the grid space
      GenerateRoomGrids();

      // Thirdly, create hallway connections, find where it intercepts with rooms and make those spaces a door
      foreach (HallwayConnection connection in HallwayConnections)
      {
        // Every coordinate is multiplied by 10 to remove the decimal and make accurate comparisons in int
        float multiplier = 10f;

        // Starting position
        int x = (int)(connection.Line0.P0.x * multiplier), y = (int)(connection.Line0.P0.y * multiplier);
        // Next position to reach
        int destinationX = (int)(connection.Line0.P1.x * multiplier), destinationY = (int)(connection.Line0.P1.y * multiplier);
        // Final positions to reach
        int finalX = connection.IsLShaped ? (int)(connection.Line1.P1.x * multiplier) : destinationX;
        int finalY = connection.IsLShaped ? (int)(connection.Line1.P1.y * multiplier) : destinationY;

        // Connection diameter
        float radius = connection.Line0.Radius;

        while (x != finalX || y != finalY)
        {
          while (x != destinationX || y != destinationY)
          {
            // Find out which direction it is advancing in
            bool advanceHorizontally = (x != destinationX) ? true : false;

            if (advanceHorizontally)
            {
              AdvanceAxis(ref x, destinationX);
            }

            else if (!advanceHorizontally)
            {
              AdvanceAxis(ref y, destinationY);
            }

            // Round so it is at the top left of the grid
            int actualX = Mathf.FloorToInt((float)x / multiplier) + m_worldToGrid.x;
            int actualY = Mathf.CeilToInt((float)y / multiplier) + m_worldToGrid.y;

            int step = 0;
            int step2 = (radius % 1f == 0) ? 1 : 0;
              
            for (int i = 0; i < Mathf.CeilToInt(radius); ++i)
            {
              GridSpace space = i == Mathf.CeilToInt(radius) - 1 ? GridSpace.Wall | GridSpace.Corridor : GridSpace.Floor | GridSpace.Corridor;

              if (advanceHorizontally) 
              {
                ThickLine(actualX, actualY + step + step2, advanceHorizontally, space);
                ThickLine(actualX, actualY - step, advanceHorizontally, space);
              }

              else
              {
                ThickLine(actualX + step, actualY, advanceHorizontally, space);
                ThickLine(actualX - step - step2, actualY, advanceHorizontally, space);
              }

              ++step;
            }
          }

          if (connection.IsLShaped)
          {
            destinationX = finalX;
            destinationY = finalY;
            radius = connection.Line1.Radius;
          }
        }
      }

      VisualiseGridSpace();
    }

    private void GenerateRoomGrids()
    {
      foreach (DungeonRoom room in DungeonRooms.Values)
      {
        // Get the corner coordinates of the room
        int top = Mathf.RoundToInt(room.Position.y + room.Scale.y / 2f) + m_worldToGrid.y;
        int bottom = Mathf.RoundToInt(room.Position.y - room.Scale.y / 2f) + m_worldToGrid.y + 1;
        int left = Mathf.RoundToInt(room.Position.x - room.Scale.x / 2f) + m_worldToGrid.x;
        int right = Mathf.RoundToInt(room.Position.x + room.Scale.x / 2f) + m_worldToGrid.x - 1;

        bool requiresWallTraversal = false;

        GridSpace roomGrid = room.RoomType == RoomType.Hub ? GridSpace.Hub : GridSpace.Hallway;

        // Traverse through every grid of the room
        for (int x = left; x <= right; ++x)
        {
          for (int y = bottom; y <= top; ++y)
          {
            // Whether the traversal is at any extremes
            bool atLeft = x == left;
            bool atRight = x == right;
            bool atBottom = y == bottom;
            bool atTop = y == top;

            m_dungeonGridSpace[x, y] = roomGrid;

            // Encase room in 1 grid of wall
            if (atLeft || atRight || atBottom || atTop)
            {
              // Corners will always be walls
              bool isACorner = (atLeft && (atBottom || atTop)) || (atRight && (atBottom || atTop));
              // Check the normal of that edge if there is a wall there already
              bool hasWallAtNormal = (atLeft || atRight) &&
                ((x != 0 && (m_dungeonGridSpace[x - 1, y] & GridSpace.Wall) != 0) || 
                (x != m_dungeonGridSpace.GetLength(0) - 1 && (m_dungeonGridSpace[x + 1, y] & GridSpace.Wall) != 0)) ||
                (atTop || atBottom) &&
                ((y != 0 && (m_dungeonGridSpace[x, y - 1] & GridSpace.Wall) != 0) ||
                (y != m_dungeonGridSpace.GetLength(1) - 1 && (m_dungeonGridSpace[x, y + 1] & GridSpace.Wall) != 0));

              // Note, this check will cause missing walls at certain areas where walls are removed. 
              // To fix this, wall traversal has to be done if any space is a hub instead of a wall
              if (isACorner || !hasWallAtNormal)
              {
                m_dungeonGridSpace[x, y] |= GridSpace.Wall;
              }

              // This will not be reached if the room is not bordering any wall, if it reaches here, then wall traversal has to be conducted
              else
              {
                m_dungeonGridSpace[x, y] |= GridSpace.Floor;

                requiresWallTraversal = true;
              }
            }

            else
            {
              m_dungeonGridSpace[x, y] |= GridSpace.Floor;
            }
          }
        }

        // Traverse the walls to fix missing walls if required
        if (requiresWallTraversal)
        {
          TraverseWalls(top, right, bottom, left, roomGrid);
        }
      }
    }

    private void TraverseWalls(int top, int right, int bottom, int left, GridSpace roomGrid)
    {
      // Move in order of axes down -> left -> up -> right
      // Naming is unnecessary and is just there for readability
      int downAxis = 0, leftAxis = 1, upAxis = 2, rightAxis = 3, completed = 4;
      int currentAxis = downAxis;
      int previousAxis = rightAxis;

      // Therefore, start at the top right
      Vector2Int currentPos = new Vector2Int(right, top);

      // Traverse until completed a full round
      while (currentAxis != completed)
      {
        int xMove = (currentAxis - 2) * (currentAxis % 2);
        int yMove = (currentAxis - 1) * ((currentAxis + 1) % 2);

        int xPreviousMove = (previousAxis - 2) * (previousAxis % 2);
        int yPreviousMove = (previousAxis - 1) * ((previousAxis + 1) % 2);

        int destinationX = xMove > 0 ? right : xMove < 0 ? left : currentPos.x;
        int destinationY = yMove > 0 ? top : yMove < 0 ? bottom : currentPos.y;

        // Switches between 1 and -1 everytime we move in a previous axis, reset every time we enter a new axis
        int previousAxisMultiplier = 1;

        // Within the curret axis
        while ((xMove != 0 && currentPos.x != destinationX) || ((yMove != 0) && currentPos.y != destinationY))
        {
          Vector2Int checkPos = new Vector2Int(currentPos.x + xMove, currentPos.y + yMove);

          // If the next slot has no wall
          if ((m_dungeonGridSpace[checkPos.x, checkPos.y] & GridSpace.Wall) == 0)
          {
            // Find the slot in the direction of the previous axis
            int checkPreviousAxisX = currentPos.x + xPreviousMove * previousAxisMultiplier;
            int checkPreviousAxisY = currentPos.y + yPreviousMove * previousAxisMultiplier;

            // If it has no wall as well
            if ((m_dungeonGridSpace[checkPreviousAxisX, checkPreviousAxisY] & GridSpace.Wall) == 0)
            {
              // If it is in previous axis
              if (previousAxisMultiplier > 0)
              {
                // Create wall in current axis' next slot and advance there
                m_dungeonGridSpace[checkPos.x, checkPos.y] = roomGrid | GridSpace.Wall;

                currentPos = checkPos;
              }

              // If it is opposite of previous axis
              else
              {
                // Create wall in opposite of previous axis slot
                m_dungeonGridSpace[checkPreviousAxisX, checkPreviousAxisY] = roomGrid | GridSpace.Wall;
              }
            }

            // Regardless of previous results, advance one step in the previous axis (opposite or not opposite)
            // Find the slot in the direction of the previous axis
            int previousAxisX = currentPos.x + xPreviousMove * previousAxisMultiplier;
            int previousAxisY = currentPos.y + yPreviousMove * previousAxisMultiplier;

            currentPos.x = previousAxisX;
            currentPos.y = previousAxisY;

            // This slot should be a wall, if not, throw an error
            if ((m_dungeonGridSpace[currentPos.x, currentPos.y] & GridSpace.Wall) == 0)
            {
              Debug.LogError("Wall traversal moved into a non-wall space!");

              break;
            }

            // Set this so that everytime it moves in a previous axis, it moves back and forth in that axis
            previousAxisMultiplier *= -1;
          }

          // Otherwise simply advance in the check direction
          else
          {
            currentPos = checkPos;
          }
        }

        ++currentAxis;
        previousAxis = previousAxis == rightAxis ? downAxis : previousAxis + 1;
      }
    }

    private void AdvanceAxis(ref int axis, int destinationAxis)
    {
      int maxStep = 10;
      int diff = Mathf.Abs(axis - destinationAxis);
      int step = Mathf.Min(maxStep, diff);

      axis += axis < destinationAxis ? step : -step;
    }

    private void ThickLine(int x, int y, bool horizontalLine, GridSpace newGrid)
    {
      if (m_dungeonGridSpace[x, y] == GridSpace.Empty)
      {
        m_dungeonGridSpace[x, y] = newGrid;
      }
    }

    private void VisualiseGridSpace()
    {
      for (int x = 0; x < m_dungeonGridSpace.GetLength(0); ++x)
      {
        for (int y = 0; y < m_dungeonGridSpace.GetLength(1); ++y)
        {
          if (m_dungeonGridSpace[x, y] != GridSpace.Empty)
          {
            Color gridColour = (m_dungeonGridSpace[x, y] & GridSpace.Hub) != 0 ? Color.red :
              (m_dungeonGridSpace[x, y] & GridSpace.Hallway) != 0 ? Color.blue :
              (m_dungeonGridSpace[x, y] & GridSpace.Corridor) != 0 ? Color.yellow : Color.black;

            GameObject primitive = null;

            if ((m_dungeonGridSpace[x, y] & GridSpace.Floor) != 0)
            {
              primitive = GameObject.CreatePrimitive(PrimitiveType.Quad);
              primitive.transform.Rotate(90f, 0, 0);
            }

            else if ((m_dungeonGridSpace[x, y] & GridSpace.Wall) != 0)
            {
              primitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            }

            primitive.transform.position = new Vector3(x + 0.5f - m_worldToGrid.x, 3f, y - 0.5f - m_worldToGrid.y);
            primitive.GetComponent<MeshRenderer>().material.color = gridColour;
          }
        }
      }
    }
  }
}
