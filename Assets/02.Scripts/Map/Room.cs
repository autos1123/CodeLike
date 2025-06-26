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

public class Room : MonoBehaviour
{
    public int Id {  get; private set; }
    public Vector2Int GridPosition { get; private set; } // 생성 시 사용한 그리드 위치
    public RoomType Type { get; private set; }

    public List<RoomConnection> Connections { get; private set; } = new();

    public void Initialize(int id, Vector2Int gridPos, RoomType type)
    {
        Id = id;
        GridPosition = gridPos;
        Type = type;
    }

    public void AddConnection(RoomConnection conn)
    {
        Connections.Add(conn);
    }
}
