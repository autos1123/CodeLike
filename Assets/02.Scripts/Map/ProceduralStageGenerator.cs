using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageGenerator:MonoBehaviour
{
    //맵 생성 시드 번호
    public int seed;
    //맵 랜덤화 함수 참조항목
    public System.Random random;
     //방 갯수(StageManager에서 랜덤화됨)
    public int roomCount;

    //그리드 반경 및 높이
    public int gridWidth = 10;
    public int gridHeight = 10;

    //다음 방 ID
    public int nextRoomID;

    //연결지점 및 방 프리팹들
    public GameObject connectionPrefab;

    public RoomPrefabSet prefabSet;

    //방을 생성할 좌표(오브젝트)
    public Transform roomParent;

    private bool[,] grid;
    public StageData stageData;
    public List<Room> AllRooms { get; private set; } = new();

    private bool isShopRoomSpawned;
    private int shopRoomPlacementOrder;

    public List<Room> Generate(int seed)
    {
        this.seed = seed;
        random = new System.Random(seed);
        nextRoomID = 0; 
        grid = new bool[gridWidth, gridHeight];

        stageData = new StageData();
        stageData.InitializeGrid(gridWidth, gridHeight);

        isShopRoomSpawned = false;
        shopRoomPlacementOrder = random.Next(1, roomCount - 1);
        
        Vector2Int startGridPos = new Vector2Int(0, 0);
        Stack<Vector2Int> stack = new();
        Dictionary<Vector2Int, int> roomIdMap = new();

        stack.Push(startGridPos);

        while(roomIdMap.Count < roomCount && stack.Count > 0)
        {

            Vector2Int current = stack.Pop();
            if(grid[current.x, current.y]) continue;

            RoomType type = RoomType.Normal;
            if(roomIdMap.Count == 0) type = RoomType.Start;
            else if(roomIdMap.Count == roomCount - 1) type = RoomType.Boss;
            else if(roomIdMap.Count == shopRoomPlacementOrder && !isShopRoomSpawned)
            {
                type = RoomType.Shop;
            }
            List<Direction> connectedDirs = new();
            foreach(var dir in GetShuffledDirections())
            {
                Vector2Int neighbor = current + DirectionToOffset(dir);
                if(IsInBounds(neighbor) && grid[neighbor.x, neighbor.y])
                {
                    int neighborId = roomIdMap[neighbor];
                    RoomConnection conn = new RoomConnection(nextRoomID, neighborId, dir);
                    stageData.connections.Add(conn);
                    connectedDirs.Add(dir);

                }
            }

            Room room = CreateRoom(current, type);
            roomIdMap[current] = room.Id;
            stageData.RegisterRoom(room);
            grid[current.x, current.y] = true;

            foreach(var dir in GetShuffledDirections())
            {
                Vector2Int next = current + DirectionToOffset(dir);
                if(IsInBounds(next) && !grid[next.x, next.y])
                {
                    stack.Push(next);
                }
            }

            // 삭제해도 될 조건문
            if(grid[current.x, current.y])
            {
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
            Debug.LogError($"❌ 프리팹이 존재하지 않음! RoomType: {type}");
            return null;
        }

        int gridSpacing = 250;
        Vector3 worldPos = new Vector3(gridPos.x * gridSpacing, gridPos.y * gridSpacing, 0f);

  
        GameObject roomGO = Instantiate(prefab, worldPos, Quaternion.identity, roomParent);
        Room room = roomGO.GetComponent<Room>();
        if(room == null)
        {
            Debug.LogError($"❌ Room 컴포넌트가 프리팹 '{prefab.name}'에 없음!");
            return null;
        }


        room.Initialize(nextRoomID++, gridPos, type);

       // room.SetRoomActive(false);

        AllRooms.Add(room);

        return room;
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    private List<Direction> GetShuffledDirections()
    {
        List<Direction> dirs = new() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        for(int i = 0; i < dirs.Count; i++)
        {
            int j = random.Next(i, dirs.Count);
            (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
        }
        return dirs;
    }

    private Vector2Int DirectionToOffset(Direction dir)
    {
        return dir switch
        {
            Direction.Up => new Vector2Int(0, 1),
            Direction.Down => new Vector2Int(0, -1),
            Direction.Left => new Vector2Int(-1, 0),
            Direction.Right => new Vector2Int(1, 0),
            _ => Vector2Int.zero,
        };
    }
    public void PlaceConnections()
    {

        foreach(var conn in stageData.connections)
        {
            if (!stageData.roomMap.TryGetValue(conn.FromRoomID, out var fromRoom) ||
                !stageData.roomMap.TryGetValue(conn.ToRoomID, out var toRoom))
                continue;

            fromRoom.AddConnection(conn);
            toRoom.AddConnection(new RoomConnection(conn.ToRoomID, conn.FromRoomID, Room.GetOppositeDirection(conn.Direction)));


            CreatePortal(fromRoom, toRoom, conn.Direction);
            CreatePortal(toRoom, fromRoom, Room.GetOppositeDirection(conn.Direction));
        }
    }

    private void CreatePortal(Room fromRoom, Room toRoom, Direction direction)
    {
        Transform fromAnchor = fromRoom.GetEntranceAnchor(direction);
        //Transform toAnchor = toRoom.GetEntranceAnchor(Room.GetOppositeDirection(direction));
        Transform toAnchor = toRoom.GetSponPos();

        if(fromAnchor == null || toAnchor == null)
        {
            Debug.LogWarning($"Missing anchor for Room {fromRoom.Id} → {toRoom.Id} at direction {direction}");
            return;
        }

        GameObject portalGO = Instantiate(connectionPrefab, fromAnchor.position, fromAnchor.rotation, fromRoom.transform);
        Portal portal = portalGO.GetComponent<Portal>();
        if(portal != null)
        {
            portal.destinationPoint = toAnchor;
            portal.exitDirection = direction; // ✅ 여기 수정!
            portal.destinationRoom = toRoom;
        }
    }


}

