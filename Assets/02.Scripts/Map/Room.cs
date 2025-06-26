using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Boss,
    Shop,
    Event
}

public class Room
{
    //필드
    public int Id { get; private set; }
    public Vector2Int Position { get; private set; }
    public Vector2Int Size { get; private set; }
    public RoomType Type { get; set; }
    public List<RoomConnection> Connections { get; private set; } = new();

    public Room(int id, Vector2Int position, Vector2Int size, RoomType type)
    {
        Id = id;
        Position = position;
        Size = size;
        Type = type;
    }

    public int GetLeftEdge() => Position.x;
    public int GetRightEdge() => Position.x + Size.x;
    public void AddConnection(RoomConnection conn) => Connections.Add(conn);
 }
