using UnityEngine;

public enum RoomType
{
  Hub = 0,
  Hallway
}

public class DungeonRoom
{
  public int Id { get; }
  public Vector2 Position { get; }
  public Vector2 Scale { get; }
  public RoomType RoomType { get; }

  public DungeonRoom(int id, Vector2 position, Vector2 scale, RoomType roomType)
  {
    Id = id;
    Position = position;
    Scale = scale;
    RoomType = roomType;
  }
}
