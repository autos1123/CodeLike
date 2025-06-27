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
    public Transform playerSpawnPoint;

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

    public Vector3 GetPlayerSpawnPoint()
    {
        return playerSpawnPoint != null  ? playerSpawnPoint.position : transform.position; 
    }

    public MinimapRoomData GetMinimapData()
    {
        var data = new MinimapRoomData
        {
            roomID = Id,
            worldPosition = transform.position,
            type = Type,
            connectedDirections = new List<Direction>()
        };

        foreach (var conn in Connections)
        {
            if (conn.FromRoomID == Id)
                data.connectedDirections.Add(conn.Direction);
            else if(conn.ToRoomID == Id)
                data.connectedDirections.Add(GetOppositeDirection(conn.Direction));
        }

        return data;
    }

    private Direction GetOppositeDirection(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => dir
        };
    }
}
