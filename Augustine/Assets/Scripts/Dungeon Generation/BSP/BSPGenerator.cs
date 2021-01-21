using System.Collections.Generic;
using UnityEngine;

namespace BSPGeneration
{
  public class BSPGenerator : MonoBehaviour
  {
    [SerializeField]
    private Terrain m_dungeonSpace = null;

    private void Awake()
    {
      // TODO: Randomise terrain width and length

      // Create BSP Tree 
      BSPTree tree = new BSPTree();

      tree.GenerateBSPDungeon(m_dungeonSpace.terrainData.size);
    }
  }
}
