using UnityEngine;

namespace BSPGeneration
{
  public class BSPNode
  {
    public BSPNode ParentNode = null;
    public BSPNode LeftNode = null;
    public BSPNode RightNode = null;

    public int Depth = 0;

    public Rect RoomRect;

    public BSPNode(BSPNode parentNode, int nodeLevel, Rect roomRect)
    {
      ParentNode = parentNode;
      Depth = nodeLevel;
      RoomRect = roomRect;
    }

    public void PartitionNode(float minRoomDimension, float maxRoomArea)
    {
      float currentNodeSize = RoomRect.width * RoomRect.height;

      // Partition the room
      if (currentNodeSize > maxRoomArea)
      {
        bool partitionHorizontally = false;

        // Check each dimension if it is <= the min room dimension * 2, which means it has no space to be split in that dimension
        // If it is then choose the other dimension to partition
        if (RoomRect.width <= minRoomDimension * 2f)
        {
          // Partition horizontally (cut y)
          partitionHorizontally = true;
        }

        else if (RoomRect.height <= minRoomDimension * 2f)
        {
          // Partition vertically (cut x)
          partitionHorizontally = false;
        }

        // Randomise the direction to partition (horizontal cut y or vertical cut x)
        else
        {
          partitionHorizontally = Random.value > 0.5f;
        }

        Rect leftRect;
        Rect rightRect;

        // Cut y
        if (partitionHorizontally)
        {
          float yCut = Random.Range(RoomRect.yMin + minRoomDimension, RoomRect.yMax - minRoomDimension);

          leftRect = new Rect(RoomRect.xMin, RoomRect.yMin, RoomRect.width, yCut - RoomRect.yMin);
          rightRect = new Rect(RoomRect.xMin, yCut, RoomRect.width, RoomRect.yMax - yCut);
        }

        else
        {
          float xCut = Random.Range(RoomRect.xMin + minRoomDimension, RoomRect.xMax - minRoomDimension  );

          leftRect = new Rect(RoomRect.xMin, RoomRect.yMin, xCut - RoomRect.xMin, RoomRect.height);
          rightRect = new Rect(xCut, RoomRect.yMin, RoomRect.xMax - xCut, RoomRect.height);
        }

        LeftNode = new BSPNode(this, Depth + 1, leftRect);
        RightNode = new BSPNode(this, Depth + 1, rightRect);

        LeftNode.PartitionNode(minRoomDimension, maxRoomArea);
        RightNode.PartitionNode(minRoomDimension, maxRoomArea);
      }

      // DEBUG, instantiate rooms as cubes in the scene
      else
      {
        GameObject room = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float randomWidth = Random.Range(RoomRect.width * 0.5f, RoomRect.width * 0.95f);
        float randomHeight = Random.Range(RoomRect.height * 0.5f, RoomRect.height * 0.95f);
        room.transform.localScale = new Vector3(randomWidth, 0, randomHeight);

        float widthPadding = RoomRect.width - randomWidth;
        float heightPadding = RoomRect.height - randomHeight;

        float widthOffset = Random.Range(-widthPadding / 2f, widthPadding / 2f);
        float heightOffset = Random.Range(-heightPadding / 2f, heightPadding / 2f);
        room.transform.position = new Vector3(RoomRect.xMin + RoomRect.width / 2f + widthOffset, 0f, RoomRect.yMin + RoomRect.height / 2f + heightOffset);
      }
    }
  }
}