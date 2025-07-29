using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

[System.Serializable]
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
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform entranceUp;
    [SerializeField] private Transform entranceDown;
    [SerializeField] private Transform entranceLeft;
    [SerializeField] private Transform entranceRight;

    public List<Portal> Portals { get; private set; } = new List<Portal>();

    public bool isClearRoom { get; private set; } = false;

    [SerializeField] [CanBeNull] private GameObject[] Enumys;
    private int enemyCount;

    public List<RoomConnection> Connections { get; private set; } = new();

    private IEnumerator Start()
    {
        var surface = GetComponent<NavMeshSurface>();
        if(surface != null)
        {
            surface.BuildNavMesh();
            yield return null;
        }

        if(Enumys != null)
        {
            Debug.Log("dd");
            foreach(var item in Enumys)
            {
                item.SetActive(true);
            }

            if(Enumys.Count() == 0)
            {
                StartCoroutine(RoomClear());
            }

            enemyCount = Enumys.Length;
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
        enemyCount--;

        if(enemyCount > 0) return;

        StartCoroutine(RoomClear());
    }
    public IEnumerator RoomClear()
    {
        yield return new WaitForSeconds(1f);
        isClearRoom = true;
        for(int i = 0; i < Portals.Count; i++)
        {
            Portals[i].OnPotalActivated();
        }
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
}
