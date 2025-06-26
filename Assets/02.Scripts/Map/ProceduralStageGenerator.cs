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

    public Room CreateRoom(Vector2Int gridPos)
    {
        int width = random.Next(minRoomSize.x, maxRoomSize.x + 1);
        int height = random.Next(minRoomSize.y, maxRoomSize.y + 1);
        int x = random.Next(0, 100);
        int y = random.Next(0, 10);

        int gridSpacing = 20;
        Vector2Int position = new Vector2Int(gridPos.x * gridSpacing, gridPos.y * gridSpacing);
        Vector2Int size = new Vector2Int(width, height);

        Room room = new Room(nextRoomID++, position, size, RoomType.Normal);
        return room;
    } 

    public bool AreRoomsOverlapping(Room roomA, Room roomB)
    {
        int ax1 = roomA.Position.x;
        int ay1 = roomA.Position.y;
        int ax2 = ax1 + roomA.Size.x;
        int ay2 = ay1 + roomA.Size.y;

        int bx1 = roomB.Position.x;
        int by1 = roomB.Position.y;
        int bx2 = bx1 + roomB.Size.x;
        int by2 = by1 + roomB.Size.y;

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
        Vector2Int[,] gridCoords = new Vector2Int[gridWidth, gridHeight];

        Vector2Int startGridPos = new Vector2Int(0, 1);
        Vector2Int current = startGridPos;

        for (int i = 0; i < roomCount; i++)
        {
            Room room = CreateRoom(current);
            if(i == 0)
                room.Type = RoomType.Start;
            else if(i == roomCount - 1)
                room.Type = RoomType.Boss;

            rooms.Add(room);
            grid[current.x, current.y] = true;
            gridCoords[current.x, current.y] = room.Position;

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
