using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
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

    [Header("Anchor")]
    public Transform playerSpawnPoint;
    public Transform entranceUp;
    public Transform entranceDown;
    public Transform entranceLeft;
    public Transform entranceRight;

    public bool isClearRoom = false;

    public event Action onRoomClear;

    public GameObject[] Enumys;
    public List<RoomConnection> Connections { get; private set; } = new();

    private IEnumerator Start()
    {
        var surface = GetComponent<NavMeshSurface>();
        if(surface != null)
        {
            surface.BuildNavMesh();
            yield return null;
        }
        foreach(var item in Enumys)
        {
            item.SetActive(true);
        }
        if(Enumys.Count() == 0)
        {
            StartCoroutine(RoomClear());
        }
    }

    public void Initialize(int id, Vector2Int gridPos, RoomType type)
    {
        Id = id;
        GridPosition = gridPos;
        Type = type;
    }
    public void ChackClear()
    {
        foreach(var item in Enumys)
        {
            if(item.activeInHierarchy == true) return;
        }
        StartCoroutine(RoomClear());
    }
    public IEnumerator RoomClear()
    {
        yield return new WaitForSeconds(1f);
        isClearRoom = true;
        onRoomClear?.Invoke();
    }
    public void AddConnection(RoomConnection conn)
    {
        Connections.Add(conn);
    }

    public Vector3 GetPlayerSpawnPoint()
    {
        return playerSpawnPoint != null  ? playerSpawnPoint.position : transform.position; 
    }

    public void SetRoomActive(bool isactive)
    {
        gameObject.SetActive(isactive);
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

    public static Direction GetOppositeDirection(Direction dir)
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
    public Transform GetEntranceAnchor(Direction dir)
    {
        return dir switch
        {
            Direction.Up => entranceUp,
            Direction.Down => entranceDown,
            Direction.Left => entranceLeft,
            Direction.Right => entranceRight,
            _ => null
        };
    }

    public Transform GetSponPos()
    {
        return playerSpawnPoint;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetCurrentRoom(this);
            }
        }
    }
}
