using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData 
{
    public int stageID;
    public Room[,] roomGrid;
    public List<RoomConnection> connections = new();
    public Dictionary<int, Room> roomMap = new();

    public Vector3 playerSpawnPoint;
    public Room startRoom { get; private set; }
    public Room bossRoom { get; private set; }

    public void InitializeGrid(int width, int height)
    {
        roomGrid = new Room[width, height];
    }

    public void RegisterRoom(Room room)
    {
        var pos = room.GridPosition;

        if(pos.x < 0 || pos.x >= roomGrid.GetLength(0) || pos.y < 0 || pos.y >= roomGrid.GetLength(1))
        {
            Debug.LogError($"❌ Room 위치가 범위를 벗어남: {pos}");
            return;
        }

        Debug.Log($"📌 RegisterRoom 호출: ID={room.Id}, Type={room.Type}, Pos={pos}");

        roomGrid[pos.x, pos.y] = room;
        roomMap[room.Id] = room;

        if(room.Type == RoomType.Start)
            startRoom = room;
        else if(room.Type == RoomType.Boss)
            bossRoom = room;
    }
}
public static class MinimapBuilder
{
    public static List<MinimapRoomData> BuildFromStage(StageData stageData, List<RoomConnection> connections)
    {
        var minimapRooms = new List<MinimapRoomData>();
        var grid = stageData.roomGrid;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var room = grid[x, y]; 
                if (room == null) continue;

                var data = room.GetMinimapData();
                minimapRooms.Add(data);
            }
        }
        return minimapRooms;
    }
}
