//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ProceduralStageGenerator : MonoBehaviour
//{
//    public int seed;
//    public System.Random random;
//    public List<Room> rooms;
//    public List<RoomConnection> connections;
//    public int roomCount;
//    public Vector2Int minRoomSize;
//    public Vector2Int maxRoomSize;
//    public int nextRoomID;

//    public RoomPrefabSet prefabSet;
//    public Transform roomParent;

//    public Room CreateRoom(Vector2Int gridPos, RoomType type) //룸 생성 함수
//    {
//        int gridSpacing = 20;
//        Vector3 worldPos = new Vector3(gridPos.x * gridSpacing, gridPos.y * gridSpacing, 0f); 

//        GameObject prefab = prefabSet.GetRandomPrefab(type);
//        GameObject roomGO = Instantiate(prefab, worldPos, Quaternion.identity, roomParent);

//        Room room = roomGO.GetComponent<Room>();
//        room.Initialize(nextRoomID++, gridPos, type);
//        return room;
//    } 


//    public void ConnectRooms() //룸 연결 함수(RoomConnection과 연결)
//    {
//        for (int i = 1; i < rooms.Count; i++)
//        {
//            Room prev = rooms[i - 1];
//            Room curr = rooms[i];



//            Direction direction; //룸의 방향을 기준으로 연결

//            Vector2Int diff = curr.GridPosition - prev.GridPosition;
//            if (diff.x == 1) direction = Direction.Right;
//            else if (diff.x == -1) direction = Direction.Left;
//            else if (diff.y == 1) direction = Direction.Up;
//            else direction = Direction.Down;

//            RoomConnection connection = new RoomConnection 
//                (
//                    fromRoomID: prev.Id,
//                    toRoomID: curr.Id,
//                    direction: direction
//                );

//            prev.AddConnection(connection);
//            connections.Add(connection);
//        }
//    }

//    public bool AreRoomsOverlapping(Room roomA, Room roomB) //룸이 겹치는지 테스트하는 함수
//    {
//        Vector3 apos = roomA.transform.position;
//        Vector3 bpos = roomB.transform.position;

//        // 프리팹의 실제 크기 값 (예: BoxCollider, SpriteRenderer 크기 등)에서 구하거나, 하드코딩된 RoomSize를 임시로 사용
//        float width = 10f;
//        float height = 6f;

//        float ax1 = apos.x;
//        float ay1 = apos.z; // Y축이 아니라 Z축 기준 (3D 공간 상에서)
//        float ax2 = ax1 + width;
//        float ay2 = ay1 + height;

//        float bx1 = bpos.x;
//        float by1 = bpos.z;
//        float bx2 = bx1 + width;
//        float by2 = by1 + height;

//        return !(ax2 <= bx1 || ax1 >= bx2 || ay2 <= by1 || ay1 >= by2);

//    }

//    public List<Room> Generate(int seed) //실제 절차적 생성 함수
//    {
//        this.seed = seed;
//        random = new System.Random(seed);
//        rooms = new List<Room>();
//        connections = new List<RoomConnection>();
//        nextRoomID = 0;

//        int gridWidth = 5;
//        int gridHeight = 5;
//        bool[,] grid = new bool[gridWidth, gridHeight];

//        Vector2Int startGridPos = new Vector2Int(0, 1);
//        Vector2Int current = startGridPos;

//        for (int i = 0; i < roomCount; i++)
//        {
//            RoomType type = RoomType.Normal;
//            if (i == 0) type = RoomType.Start;
//            else if (i == roomCount - 1) type = RoomType.Boss;

//            Room room = CreateRoom(current, type);
//            rooms.Add(room);
//            grid[current.x, current.y] = true;

//                //다음 이동 방향 결정: 오른쪽 > 아래  > 위 > 왼쪽
//                List<Vector2Int> directions = new()
//            {
//                new Vector2Int(1, 0), //오른쪽
//                new Vector2Int(0, -1), //아래쪽
//                new Vector2Int(0, 1), //위쪽 
//                new Vector2Int(-1, 0) //왼쪽
//            };

//            directions.Shuffle(random);

//            bool moved = false;
//            foreach (var dir in directions)
//            {
//                Vector2Int next = current + dir;
//                if (next.x >= 0 && next.x < gridWidth && next.y >= 0 && next.y < gridHeight && !grid[next.x, next.y])
//                {
//                    current = next;
//                    moved = true;
//                    break;
//                }
//            }

//            if(!moved) break;

//        }
//        ConnectRooms(); //룸 연결
//        return rooms; //값 반환
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageGenerator:MonoBehaviour
{
    public int seed;
    public System.Random random;
    public List<Room> rooms;
    public List<RoomConnection> connections;
    public int roomCount;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public int nextRoomID;

    public RoomPrefabSet prefabSet;
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
}

