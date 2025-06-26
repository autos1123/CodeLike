using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageGenerator : MonoBehaviour
{
    public int seed;
    public System.Random random;
    public List<Room> rooms;
    public List<RoomConnection> connections;
    public int roomCount;
    public Vector2Int minRoomSize;
    public Vector2Int maxRoomSize;
    public int nextRoomID;

    public RoomPrefabSet prefabSet;
    public Transform roomParent;

    public Room CreateRoom(Vector2Int gridPos, RoomType type)
    {
        int gridSpacing = 20;
        Vector3 worldPos = new Vector3(gridPos.x *  gridSpacing, 0f, gridPos.y * gridSpacing);

        GameObject prefab = prefabSet.GetRandomPrefab(type);
        GameObject roomGO = Instantiate(prefab, worldPos, Quaternion.identity, roomParent);

        Room room = roomGO.GetComponent<Room>();
        room.Initialize(nextRoomID++, gridPos, type);
        return room;
    } 

    public bool AreRoomsOverlapping(Room roomA, Room roomB)
    {
        Vector3 apos = roomA.transform.position;
        Vector3 bpos = roomB.transform.position;

        //프리팹 실제 크기값
        float width = 10f;
        float height = 6f;

        float ax1 = apos.x;
        float ay1 = apos.y;
        float ax2 = ax1 + width;   
        float ay2 = ay1 + height;

        float bx1 = bpos.x;
        float by1 = bpos.y;
        float bx2 = bx1 + width;  
        float by2 = by1 + height;

        return !(ax2 <= bx1 || ax1 >= bx2 || ay2 <= by1 || ay1 >= by2);

    }

    public List<Room> Generate(int seed)
    {
        this.seed = seed;
        random = new System.Random(seed);
        rooms = new List<Room>();
        nextRoomID = 0;

        int gridWidth = 5;
        int gridHeight = 5;
        bool[,] grid = new bool[gridWidth, gridHeight];

        Vector2Int startGridPos = new Vector2Int(0, 1);
        Vector2Int current = startGridPos;

        for (int i = 0; i < roomCount; i++)
        {
            RoomType type = RoomType.Normal;
            if (i == 0) type = RoomType.Start;
            else if (i == roomCount - 1) type = RoomType.Boss;

            Room room = CreateRoom(current, type);
            rooms.Add(room);
            grid[current.x, current.y] = true;

                //다음 이동 방향 결정: 오른쪽 > 아래  > 위 > 왼쪽
                List<Vector2Int> directions = new()
            {
                new Vector2Int(1, 0), //오른쪽
                new Vector2Int(0, -1), //아래쪽
                new Vector2Int(0, 1), //위쪽 
                new Vector2Int(-1, 0) //왼쪽
            };

            directions.Shuffle(random);

            bool moved = false;
            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (next.x >= 0 && next.x < gridWidth && next.y >= 0 && next.y < gridHeight && !grid[next.x, next.y])
                {
                    current = next;
                    moved = true;
                    break;
                }
            }

            if(!moved) break;

        }
        return rooms;
    }
}
