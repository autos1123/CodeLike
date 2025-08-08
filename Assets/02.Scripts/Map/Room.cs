using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[System.Serializable]
public enum RoomType
{
    Start,
    End,
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

    private Transform enemyContainer;
    private int enemyCount;

    public List<RoomConnection> Connections { get; private set; } = new();

    private Coroutine guideCoroutine;

    private void OnEnable()
    {
        if(isClearRoom)
        {
            foreach(var portal in GetComponentsInChildren<Portal>())
            {
                portal.OnPotalActivated(); // 파티클 & 콜라이더 다시 켜기
            }

            if(guideCoroutine != null)
            {
                StopCoroutine(guideCoroutine);
            }

            guideCoroutine = StartCoroutine(RepeatGuide());
        }
    }

    private void OnDisable()
    {
        if(guideCoroutine != null)
        {
            StopCoroutine(guideCoroutine);
            guideCoroutine = null;
        }
    }

    private IEnumerator Start()
    {
        var surface = GetComponent<NavMeshSurface>();
        if(surface != null)
        {
            surface.BuildNavMesh();
            yield return null;
        }

        enemyContainer = transform.Find("Enemies");

        if(enemyContainer != null)
        {
            for(int i = 0; i < enemyContainer.childCount; i++)
            {
                enemyContainer.GetChild(i).gameObject.SetActive(true);
            }

            if(enemyContainer.childCount == 0)
            {
                StartCoroutine(RoomClear());
            }

            enemyCount = enemyContainer.childCount;
        }
    }

    public void Initialize(int id, Vector2Int gridPos, RoomType type)
    {
        Id = id;
        GridPosition = gridPos;
        Type = type;
    }
    public void CheckClear()
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

        if(guideCoroutine != null)
        {
            StopCoroutine(guideCoroutine);
        }

        guideCoroutine = StartCoroutine(RepeatGuide());
    }

    private void StartGuideToPortal()
    {
        if(Portals.Count > 0)
        {
            Vector3 pivot = GameManager.Instance.Player.GetComponent<PlayerController>().col.bounds.center;

            for(int i = 0; i < Portals.Count; i++)
            {
                GuideTrail guide = PoolManager.Instance.GetObject(PoolType.GuideTrail).GetComponent<GuideTrail>();

                Vector3 portalPos = Portals[i].GetComponent<Collider>().bounds.center;
                guide.Initialize(pivot, portalPos, Portals[i].DestinationRoom.isClearRoom);
            }   
        }
    }

    private IEnumerator RepeatGuide()
    {
        while(true)
        {
            StartGuideToPortal();
            yield return new WaitForSeconds(3f);
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
