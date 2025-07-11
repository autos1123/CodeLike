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

public class Room:MonoBehaviour
{
    public int Id { get; private set; }
    public Vector2Int GridPosition { get; private set; }
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

    private void Start()
    {
        GetComponent<NavMeshSurface>()?.BuildNavMesh();
        if(Enumys.Length == 0)
            StartCoroutine(RoomClear());
    }

    public void Initialize(int id, Vector2Int gridPos, RoomType type)
    {
        Id = id;
        GridPosition = gridPos;
        Type = type;
        ChackClear();
    }

    public void ChackClear()
    {
        foreach(var e in Enumys)
            if(e.activeInHierarchy)
                return;
        StartCoroutine(RoomClear());
    }

    public IEnumerator RoomClear()
    {
        yield return new WaitForSeconds(1f);
        isClearRoom = true;
        onRoomClear?.Invoke();
    }

    public void AddConnection(RoomConnection conn) => Connections.Add(conn);

    public Vector3 GetPlayerSpawnPoint() =>
        playerSpawnPoint != null ? playerSpawnPoint.position : transform.position;

    public MinimapRoomData GetMinimapData()
    {
        var data = new MinimapRoomData
        {
            roomID = Id,
            worldPosition = transform.position,
            type = Type,
            connectedDirections = new List<Direction>()
        };

        foreach(var conn in Connections)
        {
            if(conn.FromRoomID == Id)
                data.connectedDirections.Add(conn.Direction);
            else if(conn.ToRoomID == Id)
                data.connectedDirections.Add(GetOppositeDirection(conn.Direction));
        }

        return data;
    }

    public static Direction GetOppositeDirection(Direction dir) => dir switch
    {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        _ => dir,
    };

    public Transform GetEntranceAnchor(Direction dir) => dir switch
    {
        Direction.Up => entranceUp,
        Direction.Down => entranceDown,
        Direction.Left => entranceLeft,
        Direction.Right => entranceRight,
        _ => null,
    };

    public Transform GetSponPos() => playerSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerController>();
            if(pc != null)
                pc.SetCurrentRoom(this);
        }
    }

    // 방 활성화/비활성화 (풀링 시 오브젝트 할당 해제 대신 사용)
    public void SetRoomActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        // 방 비활성화 시에도 포탈은 항상 활성화
        foreach(var portal in GetComponentsInChildren<Portal>(true))
            portal.gameObject.SetActive(true);
    }
}
