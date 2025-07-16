using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageGenerator:MonoBehaviour
{
    // 변수는 기본적으로 private으로 관리

    //맵 생성 시드 번호
    //맵 랜덤화 함수 참조항목
    public System.Random random;
     //방 갯수(StageManager에서 랜덤화됨)

    //그리드 반경 및 높이
    public int gridWidth = 10;
    public int gridHeight = 10;

    //다음 방 ID
    public int nextRoomID;

    //연결지점 및 방 프리팹들
    public GameObject connectionPrefab;

    public RoomPrefabSet prefabSet;

    // 생성된 방을 담아둘 오브젝트
    public Transform roomParent;

    private bool[,] grid;
    public StageData stageData;

    private bool isShopRoomSpawned;
    private int shopRoomPlacementOrder;

    // 매개변수로 룸 개수도 받아오기
    public List<Room> Generate(int seed, int roomCount)
    {
  
        random = new System.Random(seed);
        nextRoomID = 0; // 첫 Room은 시작방으로 하기 위해 0부터 시작
        grid = new bool[gridWidth, gridHeight];

        stageData = new StageData();
        stageData.InitializeGrid(gridWidth, gridHeight); //

        isShopRoomSpawned = false;
        shopRoomPlacementOrder = random.Next(1, roomCount - 1);
        
        Vector2Int startGridPos = new Vector2Int(0, 0);
        Stack<Vector2Int> stack = new();
        Dictionary<Vector2Int, int> roomIdMap = new();
        // Dictionary<Vector2Int, Direction> 타입으로 현재 방과 연결된 직전 방 방향 저장

        stack.Push(startGridPos);

        // 지정된 룸 개수 만큼 반복
        // 스택이 비어있지 않아야함 == 직전 룸이 다음 룸 지정에 성공해야함
        while(roomIdMap.Count < roomCount && stack.Count > 0)
        {

            Vector2Int current = stack.Pop();

            // 이미 생성된 룸이 있는 경우
            if(grid[current.x, current.y]) continue;

            RoomType type = RoomType.Normal;
            if(roomIdMap.Count == 0) type = RoomType.Start;
            else if(roomIdMap.Count == roomCount - 1) type = RoomType.Boss;
            else if(roomIdMap.Count == shopRoomPlacementOrder && !isShopRoomSpawned)
            {
                type = RoomType.Shop;
            }

            // 수정 필요
            // Dictionary<Vector2Int, Direction>타입 변수로 해결
            // 이전 방과 연결
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

            // 다음 방과 연결
            foreach(var dir in GetShuffledDirections())
            {
                Vector2Int next = current + DirectionToOffset(dir);
                if(IsInBounds(next) && !grid[next.x, next.y])
                {
                    stack.Push(next);
                    // (int)dir * -1; == 반대 방향 구하기
                    // Dictionary<Vector2Int, Direction> 변수에 방향 저장
                }
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
            return null;
        }

        int gridSpacing = 250;
        Vector3 worldPos = new Vector3(gridPos.x * gridSpacing, gridPos.y * gridSpacing, 0f);

  
        GameObject roomGO = Instantiate(prefab, worldPos, Quaternion.identity, roomParent);
        Room room = roomGO.GetComponent<Room>();
        if(room == null)
        {
            return null;
        }


        room.Initialize(nextRoomID++, gridPos, type);

       // room.SetRoomActive(false);


        return room;
    }

    /// <summary>
    /// 주어진 위치가 그리드 범위 내에 있는지 확인합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    /// <summary>
    /// 방향을 무작위로 섞어서 반환합니다.
    /// </summary>
    /// <returns></returns>
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

