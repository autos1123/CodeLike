using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageGenerator:MonoBehaviour
{
    //맵 생성 시드 번호
    //맵 랜덤화 함수 참조항목
    private System.Random random;

    //연결지점 및 방 프리팹들
    [SerializeField] private GameObject connectionPrefab;

    [SerializeField] private RoomPrefabSet prefabSet;

    // 생성된 방을 담아둘 오브젝트
    [SerializeField] private Transform roomParent;

    // 임시 리팩토링 필요
    private bool isShopRoomSpawned;
    private int shopRoomPlacementOrder;

    public StageData Generate(int seed, int roomCount, int stageID)
    {
        random = new System.Random(seed);
        int gridWidth = StageManager.Instance.gridWidth;
        int gridHeight = StageManager.Instance.gridHeight;

        int currentRoomID = 0; // 첫 Room은 시작방으로 하기 위해 0부터 시작
        bool[,] grid = new bool[gridWidth, gridHeight]; // 가상의 그리드 생성

        // 스테이지 생성
        StageData stageData = new StageData(stageID);

        isShopRoomSpawned = false;
        shopRoomPlacementOrder = random.Next(1, roomCount - 1);
        
        Stack<Vector2Int> stack = new();
        Dictionary<Vector2Int, Direction> cameFrom = new();

        // 시작 그리드는 (0, 0)
        stack.Push(new Vector2Int(0, 0));

        // 지정된 룸 개수 만큼 반복
        // 스택이 비어있지 않아야함 == 직전 룸이 다음 룸 지정에 성공해야함
        while(currentRoomID < roomCount && stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            // 이미 생성된 룸이 있는 경우
            if(grid[current.x, current.y]) continue;

            RoomType type = RoomType.Normal;
            if(currentRoomID == 0) type = RoomType.Start;
            else if(currentRoomID == roomCount - 1) type = RoomType.Boss;
            else if(currentRoomID == shopRoomPlacementOrder && !isShopRoomSpawned) type = RoomType.Shop;

            // 이전 방과의 연결 생성
            // 현재 -> 이전 방향
            if(cameFrom.Count != 0)
            {
                RoomConnection conn = new RoomConnection(currentRoomID, currentRoomID - 1, cameFrom[current]);
                stageData.connections.Add(conn);
            }

            // 룸 생성 및 초기화
            Room room = CreateRoom(currentRoomID, current, type);
            stageData.RegisterRoom(room, gridWidth, gridHeight);
            grid[current.x, current.y] = true;

            foreach(Direction dir in GetShuffledDirections())
            {
                Vector2Int next = current + DirectionToOffset(dir);
                if(IsInBounds(next, gridWidth, gridHeight) && !grid[next.x, next.y])
                {
                    stack.Push(next);
                    cameFrom[next] = (Direction)((int)dir * -1); // 반대 방향 저장
                }
            }

            currentRoomID++;
        }
        PlaceConnections(stageData);

        return stageData;
    }

    private Room CreateRoom(int currentRoomId, Vector2Int gridPos, RoomType type)
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

        room.Initialize(currentRoomId, gridPos, type);

        return room;
    }

    /// <summary>
    /// 주어진 위치가 그리드 범위 내에 있는지 확인합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool IsInBounds(Vector2Int pos, int gridWidth, int gridHeight)
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
    public void PlaceConnections(StageData stageData)
    {

        foreach(RoomConnection conn in stageData.connections)
        {
            if (!stageData.roomMap.TryGetValue(conn.FromRoomID, out Room fromRoom) ||
                !stageData.roomMap.TryGetValue(conn.ToRoomID, out Room toRoom))
                continue;

            fromRoom.AddConnection(conn);
            toRoom.AddConnection(new RoomConnection(conn.ToRoomID, conn.FromRoomID, (Direction)((int)conn.Direction * -1)));


            CreatePortal(fromRoom, toRoom, conn.Direction);
            CreatePortal(toRoom, fromRoom, (Direction)((int)conn.Direction * -1));
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

