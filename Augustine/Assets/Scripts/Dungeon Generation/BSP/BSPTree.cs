using UnityEngine;

namespace BSPGeneration
{
  public class BSPTree
  {
    public BSPNode Root = null;

    private const float MIN_ROOM_DIMENSION = 25f;
    private float m_maxRoomArea = 1500f;

    private float m_terrainWidth = 0f;
    private float m_terrainLength = 0f;

    public void GenerateBSPDungeon(Vector3 terrainData)
    {
      m_terrainWidth = terrainData.x;
      m_terrainLength = terrainData.z;

      Root = new BSPNode(null, 1, new Rect(0, 0, m_terrainWidth, m_terrainLength));
      Root.PartitionNode(MIN_ROOM_DIMENSION, m_maxRoomArea);
    }
  }
}