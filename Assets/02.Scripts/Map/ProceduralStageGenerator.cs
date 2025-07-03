using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageGenerator:MonoBehaviour
{
    //맵 생성 시드 번호
    public int seed;
    //맵 랜덤화 함수 참조항목
    public System.Random random;
    //방 리스트
    public List<Room> rooms;
    //방 연결지점 리스트
    public List<RoomConnection> connections;
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

    public List<Room> Generate(int seed)
    {
        this.seed = seed;
        random = new System.Random(seed);
        rooms = new List<Room>();
        connections = new List<RoomConnection>();
        nextRoomID = 0;
        grid = new bool[gridWidth, gridHeight];

        Vector2Int startGridPos = new Vector2Int(gridWidth / 2, gridHeight / 2);

        Stack<Vector2Int> stack = new();
        Dictionary<Vector2Int, int> roomIdMap = new();

        stack.Push(startGridPos);

        while(rooms.Count < roomCount && stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            if(grid[current.x, current.y]) continue;

            RoomType type = RoomType.Normal;
            if(rooms.Count == 0) type = RoomType.Start;
            else if(rooms.Count == roomCount - 1) type = RoomType.Boss;

            List<Direction> connectedDirs = new();
            foreach(var dir in GetShuffledDirections())
            {
                Vector2Int neighbor = current + DirectionToOffset(dir);
                if(IsInBounds(neighbor) && grid[neighbor.x, neighbor.y])
                {
                    int neighborId = roomIdMap[neighbor];
                    RoomConnection conn = new RoomConnection(nextRoomID, neighborId, dir);
                    connections.Add(conn);
                    connectedDirs.Add(dir);
                }
            }

            Room room = CreateRoom(current, type);
            roomIdMap[current] = room.Id;
            rooms.Add(room);
            grid[current.x, current.y] = true;

            foreach(var dir in GetShuffledDirections())
            {
                Vector2Int next = current + DirectionToOffset(dir);
                if(IsInBounds(next) && !grid[next.x, next.y])
                {
                    stack.Push(next);
                }
            }
        }
        PlaceConnections();
        return rooms;
    }

    private Room CreateRoom(Vector2Int gridPos, RoomType type)
    {
        int gridSpacing = 250;
        Vector3 worldPos = new Vector3(gridPos.x * gridSpacing, gridPos.y * gridSpacing, 0f);

        GameObject prefab = prefabSet.GetRandomPrefab(type);
        GameObject roomGO = Instantiate(prefab, worldPos, Quaternion.identity, roomParent);

        Room room = roomGO.GetComponent<Room>();
        room.Initialize(nextRoomID++, gridPos, type);
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
        foreach(var conn in connections)
        {
            Room fromRoom = rooms.Find(r => r.Id == conn.FromRoomID);
            Room toRoom = rooms.Find(r => r.Id == conn.ToRoomID);

            if(fromRoom == null || toRoom == null) continue;

            // ▶ A → B 포탈
            CreatePortal(fromRoom, toRoom, conn.Direction);

            // ▶ B → A 포탈 (반대 방향)
            CreatePortal(toRoom, fromRoom, Room.GetOppositeDirection(conn.Direction));
        }
    }

    private void CreatePortal(Room fromRoom, Room toRoom, Direction direction)
    {
        Transform fromAnchor = fromRoom.GetEntranceAnchor(direction);
        Transform toAnchor = toRoom.GetEntranceAnchor(Room.GetOppositeDirection(direction));

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
        }
    }


}

