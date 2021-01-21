using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TriangleNet.Geometry;

using UnityEngine;

namespace DelaunayGeneration
{
  public class DelaunayGenerator : MonoBehaviour
  {
    [SerializeField]
    private float m_meanRoomDimensions = 20f, m_sdRoomDimensions = 50f;

    [SerializeField]
    private float m_meanNumberOfRooms = 45f, m_sdNumberOfRooms = 10f;

    [SerializeField]
    private float m_spawnRadius = 75f;

    // This holds the room cubes in an array, their index in the array is the actual node number of that room.
    private GameObject[] m_roomObjects = null;
    // Since the large rooms are stored as another id than their room id, this links up the delaunay id and the room id
    private Dictionary<int, int> m_delaunayToRoomId = new Dictionary<int, int>();
    // Holds every information about the dungeon rooms, their sizes, positions and connections
    private DungeonNodesData m_dungeonData = new DungeonNodesData();
    // Holds every large room in a list
    private List<GameObject> m_largeRooms = new List<GameObject>();

    private void Awake()
    {
      GenerateRooms();
    }

    private Vector2 GetRandomPointInCircle(float radius)
    {
      float t = 2 * Mathf.PI * Random.value;
      float u = Random.value + Random.value;
      float r = u > 1f ? 2f - u : u;

      return new Vector2(Mathf.Round(radius * r * Mathf.Cos(t)), Mathf.Round(radius * r * Mathf.Sin(t)));
    }

    private void GenerateRooms()
    {
      // Randomise the number of rooms in this dungeon
      int numberOfRooms = (int)(Random.value * m_sdNumberOfRooms + m_meanNumberOfRooms);
      // This is the threshold from the mean that will qualify a room as a hub, and be included in the triangulation and MST
      float largeRoomThreshold = m_meanRoomDimensions * 1.3f;

      m_roomObjects = new GameObject[numberOfRooms];

      print("Number of rooms to generate: " + numberOfRooms);

      // Generate the rooms
      for (int i = 0; i < numberOfRooms; ++i)
      {
        // Randomise dimensions of the room
        int randomWidth = (int)((Random.value * 2f - 1f) * m_sdRoomDimensions + m_meanRoomDimensions);
        int randomLength = (int)((Random.value * 2f - 1f) * m_sdRoomDimensions + m_meanRoomDimensions);

        Vector2 position = GetRandomPointInCircle(m_spawnRadius);

        m_roomObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // Normalise scale and position because rigidbodies don't work on anything large.......................
        m_roomObjects[i].transform.localScale = new Vector3(randomWidth / m_spawnRadius, 1f, randomLength / m_spawnRadius);
        m_roomObjects[i].transform.position = new Vector3(position.x / m_spawnRadius, 0, position.y / m_spawnRadius);

        // Determine if this room is large or not
        if (m_roomObjects[i].transform.localScale.x * m_spawnRadius >= largeRoomThreshold && m_roomObjects[i].transform.localScale.z * m_spawnRadius >= largeRoomThreshold)
        {
          m_largeRooms.Add(m_roomObjects[i]);

          // DEBUG
          m_roomObjects[i].GetComponent<MeshRenderer>().material.color = Color.red;
        }

        // Add rigidbody component, freeze y position and rotation so it doesn't fly, set high drag so it doesn't get flung out when pushed out
        Rigidbody roomRigidbody = m_roomObjects[i].AddComponent<Rigidbody>();
        roomRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        roomRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        roomRigidbody.drag = float.MaxValue;
      }

      // If generation doesn't create enough large rooms, redo the whole process...
      if (m_largeRooms.Count < 5)
      {
        print("Not enough rooms (" + m_largeRooms.Count + "), regenerating dungeon...");
        m_largeRooms.Clear();

        for (int i = m_roomObjects.Length - 1; i >= 0; --i)
        {
          Destroy(m_roomObjects[i]);
        }

        GenerateRooms();
      }

      // Now that the rooms are generated as cubes with rigidbody, wait for the physics engine to do the work
      else StartCoroutine(WaitForPhysics());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForPhysics()
    {
      yield return new WaitForSeconds(3f);

      RestoreScaleAndPositions();
    }

    private void RestoreScaleAndPositions()
    {
      // Restore the rooms scales and positions

      for (int i = 0; i < m_roomObjects.Length; ++i)
      {
        // Round the room positions and scale, do not account for odd scales since that might cause the rooms to overlap even more and rigidbody cannot deal with that when the scale is larger than 2 or so...
        m_roomObjects[i].GetComponent<Rigidbody>().MovePosition(new Vector3(Mathf.Round(m_roomObjects[i].transform.position.x * m_spawnRadius), 0, Mathf.Round(m_roomObjects[i].transform.position.z * m_spawnRadius)));
        m_roomObjects[i].transform.localScale = new Vector3(Mathf.Round(m_roomObjects[i].transform.localScale.x * m_spawnRadius), 1f, Mathf.Round(m_roomObjects[i].transform.localScale.z * m_spawnRadius));
      }
      
      // Now wait for physics to seperate the rooms again due to floating point error with calculations
      StartCoroutine(WaitForLargePhysics());
    }

    private IEnumerator WaitForLargePhysics()
    {
      // Initial large room seperation
      yield return new WaitForSeconds(2f);

      // Rounds the rooms again, this time correct positions on the axis if its scale is odd, otherwise freeze it
      for (int i = 0; i < m_roomObjects.Length; ++i)
      {
        m_roomObjects[i].transform.position = new Vector3(Mathf.Round(m_roomObjects[i].transform.position.x), 0, Mathf.Round(m_roomObjects[i].transform.position.z));

        // Correct positions if scale is odd, freeze that axis if scale is even so that it no longer moves out of place
        if ((int)m_roomObjects[i].transform.localScale.x % 2 == 1) m_roomObjects[i].transform.Translate(0.4f, 0, 0); 
        else m_roomObjects[i].GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionX;
        if ((int)m_roomObjects[i].transform.localScale.z % 2 == 1) m_roomObjects[i].transform.Translate(0, 0, 0.4f);
        else m_roomObjects[i].GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionZ;

      }

      // Wait for physics again
      yield return new WaitForSeconds(1f);

      // Final positions, rigidbody can now be removed
      for (int i = 0; i < m_roomObjects.Length; ++i)
      {
        // Remove the rigidbody component
        Destroy(m_roomObjects[i].GetComponent<Rigidbody>());

        Vector3 finalPosition = m_roomObjects[i].transform.position;

        finalPosition.x = (finalPosition.x % 1f < 0.6f) ? Mathf.Floor(finalPosition.x) : Mathf.Ceil(finalPosition.x);
        finalPosition.z = (finalPosition.z % 1f < 0.6f) ? Mathf.Floor(finalPosition.z) : Mathf.Ceil(finalPosition.z);

        m_roomObjects[i].transform.position = finalPosition;

        // Correct positions again if scale is odd
        if ((int)m_roomObjects[i].transform.localScale.x % 2 == 1) m_roomObjects[i].transform.Translate(0.5f, 0, 0);
        if ((int)m_roomObjects[i].transform.localScale.z % 2 == 1) m_roomObjects[i].transform.Translate(0, 0, 0.5f);
      }

      // Move on to delaunay triangulation of the large rooms
      DelaunayTriangulation();
    }

    /// <summary>
    /// https://straypixels.net/delaunay-triangulation-terrain/
    /// </summary>
    /// <param name="largeRoomCubeObjects"></param>
    private void DelaunayTriangulation()
    {
      Polygon polygon = new Polygon();

      for (int i = 0; i < m_largeRooms.Count; ++i)
      {
        polygon.Add(new Vertex(m_largeRooms[i].transform.position.x, m_largeRooms[i].transform.position.z));
        // Save the delaunay id to the actual room id
        m_delaunayToRoomId.Add(i, System.Array.IndexOf(m_roomObjects, m_largeRooms[i]));
      }

      TriangleNet.Mesh mesh = (TriangleNet.Mesh)polygon.Triangulate();

      StartCoroutine(DrawTriangulationLines(mesh));;

      MinimumSpanningTreePrim(mesh);
    }

    /// <summary>
    /// https://www.geeksforgeeks.org/prims-minimum-spanning-tree-mst-greedy-algo-5/
    /// </summary>
    /// <param name="mesh"></param>
    private void MinimumSpanningTreePrim(TriangleNet.Mesh mesh)
    {
      int numberOfVertices = mesh.vertices.Count;

      // Represents the set of vertices already included in MST, index are the same as the indexes in mesh.vertices
      bool[] mstSet = new bool[numberOfVertices];
      // The weight between the current node to all the other nodes
      float[] key = new float[numberOfVertices];
      // Stores the constructed MST
      int[] parent = new int[numberOfVertices];
      // Complete graph
      float[,] graph = new float[numberOfVertices, numberOfVertices];

      // Populate the graph with the weight of every vertice
      for (int v = 0; v < numberOfVertices; ++v)
      {
        for (int i = 0; i < numberOfVertices; ++i)
        {
          // Check if the vertices are the same, then weight is 0
          if (v == i)
          {
            graph[v, i] = 0f;
          }

          // Check if vertice i is an adjacent vertice to v by checking through every edge
          else
          {
            // If i is not adjacent to v, the weight is 0 and the value will not change
            graph[v, i] = 0f;

            foreach (Edge edge in mesh.Edges)
            {
              int adjacentVerticeIndex = -1;

              // If v is a vertice on this edge, get the index of the other vertice
              if (edge.P0 == v) adjacentVerticeIndex = edge.P1;
              else if (edge.P1 == v) adjacentVerticeIndex = edge.P0;

              // If the other index's vertice is i, this means that i is adjacent to v and should have a value
              if (adjacentVerticeIndex == i)
              {
                // Calculate the distance between v and i
                Vector2 vPos = new Vector2((float)mesh.vertices[v].x, (float)mesh.vertices[v].y);
                Vector2 iPos = new Vector2((float)mesh.vertices[i].x, (float)mesh.vertices[i].y);

                // Save it in the graph
                graph[v, i] = Vector2.Distance(vPos, iPos);

                break;
              }
            }
          }
        }
      }

      // Initialise every key as infinite
      for (int i = 0; i < numberOfVertices; ++i)
      {
        key[i] = float.MaxValue;
        mstSet[i] = false;
      }

      // Always include first vertex in MST, and make the key 0 so that it is picked as the first vertex
      key[0] = 0;
      parent[0] = -1;

      // Stores every edge in a list, remove the ones that are used in the MST later
      List<Edge> unusedEdges = mesh.Edges.ToList();

      for (int count = 0; count < numberOfVertices; ++count)
      {
        // Picks the minimum key vertex from the set of vertices not yet included in mstSet
        int minIndex = MinKey(key, mstSet);

        // Add to the set this new index
        mstSet[minIndex] = true;

        // When a new minIndex is chosen, a new edge is formed in the MST, remove that edge from the list of all edges
        // Skips the first node since that node has no parent
        if (minIndex != 0)
        {
          // Removes new edge in MST from unusedEdges
          for (int i = 0; i < unusedEdges.Count; ++i)
          {
            if ((unusedEdges[i].P0 == minIndex && unusedEdges[i].P1 == parent[minIndex]) || (unusedEdges[i].P1 == minIndex && unusedEdges[i].P0 == parent[minIndex]))
            {
              unusedEdges.RemoveAt(i);

              break;
            }
          }
        }

        // Update key values and parent index of adjacent vertices that are not included in mstSet yet
        for (int v = 0; v < numberOfVertices; ++v)
        {
          // Graph[minIndex][v] is non zero only for adjacent vertices
          // Only check with vertices that are not included in mstSet
          // Update only if this new distance is smaller than key[v]
          if (graph[minIndex, v] != 0f && mstSet[v] == false && graph[minIndex, v] < key[v])
          {
            parent[v] = minIndex;
            key[v] = graph[minIndex, v];
          }
        }
      }

      for (int v = 1; v < numberOfVertices; ++v)
      {
        Vector2 v0 = new Vector2((float)mesh.vertices[parent[v]].x, (float)mesh.vertices[parent[v]].y);
        Vector2 v1 = new Vector2((float)mesh.vertices[v].x, (float)mesh.vertices[v].y);
        m_dungeonData.AddEdge(v0, m_delaunayToRoomId[parent[v]], v1, m_delaunayToRoomId[v]);

        // Add the room with its actual room id
        m_dungeonData.AddRoom(m_delaunayToRoomId[v], m_roomObjects[m_delaunayToRoomId[v]], RoomType.Hub);
      }

      m_dungeonData.AddRoom(m_delaunayToRoomId[0], m_roomObjects[m_delaunayToRoomId[0]], RoomType.Hub);

      int numberOfUnusedEdgesToAdd = (int)(mesh.NumberOfEdges * 0.1f);

      // Adds some unused edges into the custom data structure
      for (int counter = 0; counter < numberOfUnusedEdgesToAdd; ++counter)
      {
        int randomIndex = Random.Range(0, unusedEdges.Count - counter);

        int p0Index = unusedEdges[randomIndex].P0;
        int p1Index = unusedEdges[randomIndex].P1;

        Vector2 v0 = new Vector2((float)mesh.vertices[p0Index].x, (float)mesh.vertices[p0Index].y);
        Vector2 v1 = new Vector2((float)mesh.vertices[p1Index].x, (float)mesh.vertices[p1Index].y);

        m_dungeonData.AddEdge(v0, m_delaunayToRoomId[p0Index], v1, m_delaunayToRoomId[p1Index]);

        unusedEdges.RemoveAt(randomIndex);
      }

      m_dungeonData.CreateHallwayLines();

      StopAllCoroutines();
      StartCoroutine(DrawDungeonConnections());

      StartCoroutine(m_dungeonData.AddHallwayRooms(m_roomObjects));
    }

    /// <summary>
    /// Helper function for MinimumSpanningTreePrim, finds the vertex with the lowest key
    /// from the set of vertices not yet included in mstSet
    /// </summary>
    /// <param name="key"></param>
    /// <param name="mstSet"></param>
    private int MinKey(float[] key, bool[] mstSet)
    {
      // Initialise min value
      float min = float.MaxValue;
      int min_index = -1;

      for (int v = 0; v < mstSet.Length; ++v)
      {
        if (mstSet[v] == false && key[v] < min)
        {
          min = key[v];
          min_index = v;
        }
      }

      return min_index;
    }

    private IEnumerator DrawTriangulationLines(TriangleNet.Mesh mesh)
    {
      while (true)
      {
        // Visualise the triangulation
        foreach (Edge edge in mesh.Edges)
        {
          Vertex v0 = mesh.vertices[edge.P0];
          Vertex v1 = mesh.vertices[edge.P1];
          Vector3 p0 = new Vector3((float)v0.x, 2f, (float)v0.y);
          Vector3 p1 = new Vector3((float)v1.x, 2f, (float)v1.y);

          Debug.DrawLine(p0, p1, Color.green);
        }

        yield return null;
      }
    }

    private IEnumerator DrawMinimumSpanningTree(TriangleNet.Mesh mesh, int[] parent)
    {
      while (true)
      {
        // Visualise the tree
        for (int v = 1; v < parent.Length; ++v)
        {
          Vertex v0 = mesh.vertices[parent[v]];
          Vertex v1 = mesh.vertices[v];
          Vector3 p0 = new Vector3((float)v0.x, 2f, (float)v0.y);
          Vector3 p1 = new Vector3((float)v1.x, 2f, (float)v1.y);

          Debug.DrawLine(p0, p1, Color.green);
        }

        yield return null;
      }
    }

    private IEnumerator DrawDungeonConnections()
    {
      while (true)
      {
        // Visualise the graph
        foreach (Edge edge in m_dungeonData.Edges)
        {
          Vector2 v0 = m_dungeonData.Vertices[edge.P0];
          Vector2 v1 = m_dungeonData.Vertices[edge.P1];
          Vector3 p0 = new Vector3(v0.x, 2f, v0.y);
          Vector3 p1 = new Vector3(v1.x, 2f, v1.y);

          Debug.DrawLine(p0, p1, Color.green);
        }

        // Visualise hallway connections
        foreach (HallwayConnection hallwayConnection in m_dungeonData.HallwayConnections)
        {
          Vector3 p0 = new Vector3(hallwayConnection.Line0.P0.x, 5f, hallwayConnection.Line0.P0.y);
          Vector3 p1 = new Vector3(hallwayConnection.Line0.P1.x, 5f, hallwayConnection.Line0.P1.y);

          Debug.DrawLine(p0, p1, Color.yellow);

          if (hallwayConnection.IsLShaped)
          {
            p0 = new Vector3(hallwayConnection.Line1.P0.x, 5f, hallwayConnection.Line1.P0.y);
            p1 = new Vector3(hallwayConnection.Line1.P1.x, 5f, hallwayConnection.Line1.P1.y);

            Debug.DrawLine(p0, p1, Color.yellow);
          }
        }

        yield return null;
      }
    }
  }

}