using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageGenerator:MonoBehaviour
{
    [Header("Generation Settings")]
    public int roomCount;
    public int gridWidth = 10;
    public int gridHeight = 10;

    [Header("References")]
    public RoomPrefabSet prefabSet;
    public GameObject connectionPrefab;
    public Transform roomParent;

    private System.Random random;
    private int nextRoomID;
    private bool[,] grid;

    public StageData stageData;
    public List<Room> AllRooms { get; private set; } = new();

    public List<Room> Generate(int seed)
    {
        Debug.Log($"🚀 Generate 시작! seed={seed}, roomCount={roomCount}, grid={gridWidth}×{gridHeight}");
        random = new System.Random(seed);
        nextRoomID = 0;
        AllRooms.Clear();

        // 그리드 및 StageData 초기화
        stageData = new StageData();
        stageData.InitializeGrid(gridWidth, gridHeight);
        grid = new bool[gridWidth, gridHeight];

        // DFS 스택 + ID 맵
        var stack = new Stack<Vector2Int>();
        var roomIdMap = new Dictionary<Vector2Int, int>();
        stack.Push(Vector2Int.zero);

        while(roomIdMap.Count < roomCount && stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            if(grid[current.x, current.y])
                continue;

            // 방 타입 결정
            RoomType type = RoomType.Normal;
            if(roomIdMap.Count == 0) type = RoomType.Start;
            else if(roomIdMap.Count == roomCount - 1) type = RoomType.Boss;

            // 이미 생성된 인접 방과의 연결 정보 수집
            foreach(var dir in GetShuffledDirections())
            {
                Vector2Int nb = current + DirectionToOffset(dir);
                if(IsInBounds(nb) && grid[nb.x, nb.y])
                {
                    int neighborId = roomIdMap[nb];
                    stageData.connections.Add(new RoomConnection(nextRoomID, neighborId, dir));
                }
            }

            // 방 생성 (풀링 방식) :contentReference[oaicite:0]{index=0}
            Room room = CreateRoom(current, type);
            roomIdMap[current] = room.Id;
            stageData.RegisterRoom(room);
            grid[current.x, current.y] = true;

            // 다음 후보 위치 푸시
            foreach(var dir in GetShuffledDirections())
            {
                Vector2Int next = current + DirectionToOffset(dir);
                if(IsInBounds(next) && !grid[next.x, next.y])
                    stack.Push(next);
            }

            Debug.Log($"현재 위치: {current}");
            if(grid[current.x, current.y])
            {
                Debug.Log($"이미 방문한 좌표: {current}");
                continue;
            }
        }

        PlaceConnections();
        return new List<Room>(stageData.roomMap.Values);
    }

    private Room CreateRoom(Vector2Int gridPos, RoomType type)
    {
        GameObject prefab = prefabSet.GetRandomPrefab(type);
        if(prefab == null)
        {
            Debug.LogError($"❌ 프리팹이 없습니다! RoomType={type}");
            return null;
        }

        // IPoolObject 구현체로부터 PoolType 가져와 풀에서 오브젝트 획득 :contentReference[oaicite:1]{index=1}
        var ipool = prefab.GetComponent<IPoolObject>();
        GameObject roomGO = ipool != null
            ? PoolManager.Instance.GetObject(ipool.PoolType)
            : Instantiate(prefab);
        roomGO.name = $"{prefab.name}_{nextRoomID}";
        roomGO.transform.SetParent(roomParent, false);

        // 위치·회전 초기화
        int spacing = 250;
        roomGO.transform.localPosition = new Vector3(gridPos.x * spacing, gridPos.y * spacing, 0f);
        roomGO.transform.localRotation = Quaternion.identity;

        var room = roomGO.GetComponent<Room>();
        if(room == null)
        {
            Debug.LogError($"❌ Room 컴포넌트 누락! 프리팹='{prefab.name}'");
            return null;
        }

        room.Initialize(nextRoomID++, gridPos, type);
        room.SetRoomActive(false);
        AllRooms.Add(room);

        Debug.Log($"✅ Room 생성 완료: ID={room.Id}, Type={type}, Pos={gridPos}");
        return room;
    }

    public void PlaceConnections()
    {
        foreach(var conn in stageData.connections)
        {
            if(!stageData.roomMap.TryGetValue(conn.FromRoomID, out var fromRoom) ||
                !stageData.roomMap.TryGetValue(conn.ToRoomID, out var toRoom))
            {
                Debug.LogWarning($"❌ 연결된 방 없음! From={conn.FromRoomID}, To={conn.ToRoomID}");
                continue;
            }

            fromRoom.AddConnection(conn);
            toRoom.AddConnection(new RoomConnection(conn.ToRoomID, conn.FromRoomID, Room.GetOppositeDirection(conn.Direction)));

            CreatePortal(fromRoom, toRoom, conn.Direction);
            CreatePortal(toRoom, fromRoom, Room.GetOppositeDirection(conn.Direction));
        }
    }

    private void CreatePortal(Room fromRoom, Room toRoom, Direction direction)
    {
        Transform fromAnchor = fromRoom.GetEntranceAnchor(direction);
        Transform toAnchor = toRoom.GetEntranceAnchor(Room.GetOppositeDirection(direction));
        if(fromAnchor == null || toAnchor == null)
        {
            Debug.LogWarning($"Missing anchor: {fromRoom.Id}→{toRoom.Id}, dir={direction}");
            return;
        }

        // 풀링 방식으로 포탈 생성 :contentReference[oaicite:2]{index=2}
        var ipool = connectionPrefab.GetComponent<IPoolObject>();
        GameObject portalGO = ipool != null
            ? PoolManager.Instance.GetObject(ipool.PoolType)
            : Instantiate(connectionPrefab);
        portalGO.name = $"Portal_{fromRoom.Id}_to_{toRoom.Id}";
        portalGO.transform.SetParent(StageManager.Instance.transform, false);
        portalGO.transform.position = fromAnchor.position;
        portalGO.transform.rotation = fromAnchor.rotation;

        var portal = portalGO.GetComponent<Portal>();
        if(portal != null)
        {
            portal.SetRoom(fromRoom);
            portal.destinationPoint = toAnchor;
            portal.exitDirection = direction;
        }
    }

    private bool IsInBounds(Vector2Int p) =>
        p.x >= 0 && p.x < gridWidth && p.y >= 0 && p.y < gridHeight;

    private Vector2Int DirectionToOffset(Direction d) => d switch
    {
        Direction.Up => new Vector2Int(0, 1),
        Direction.Down => new Vector2Int(0, -1),
        Direction.Left => new Vector2Int(-1, 0),
        Direction.Right => new Vector2Int(1, 0),
        _ => Vector2Int.zero,
    };

    private List<Direction> GetShuffledDirections()
    {
        var dirs = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        for(int i = 0; i < dirs.Count; i++)
        {
            int j = random.Next(i, dirs.Count);
            (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
        }
        return dirs;
    }
}

