using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData 
{
    public int stageID { get; private set; } // 현재 초기화 안됨
    public List<RoomConnection> connections { get; private set; } = new();
    public Dictionary<int, Room> roomMap { get; private set; } = new();
    public Vector3 playerSpawnPoint { get; private set; }
    public Room startRoom { get; private set; }
    public Room bossRoom { get; private set; }

    public StageData(int stageID)
    {
        this.stageID = stageID;

    }

    public void RegisterRoom(Room room, int gridWidth, int gridHeight)
    {
        var pos = room.GridPosition;

        // 룸의 그리드 좌표가 그리드를 벗어난 경우
        if(pos.x < 0 || pos.x >= gridWidth || pos.y < 0 || pos.y >= gridHeight)
        {
            return;
        }

        roomMap[room.Id] = room;

        if(room.Type == RoomType.Start)
        {
            startRoom = room;
            playerSpawnPoint = room.GetPlayerSpawnPoint();
        }
        else
        {
            if(room.Type == RoomType.Boss)
                bossRoom = room;

            room.gameObject.SetActive(false);
        }
    }
}
public static class MinimapBuilder
{
    public static List<MinimapRoomData> BuildFromStage(StageData stageData, List<RoomConnection> connections)
    {
        var minimapRooms = new List<MinimapRoomData>();
        //var grid = stageData.roomGrid;
        //int width = grid.GetLength(0);
        //int height = grid.GetLength(1);
        
        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        var room = grid[x, y]; 
        //        if (room == null) continue;

        //        var data = room.GetMinimapData();
        //        minimapRooms.Add(data);
        //    }
        //}
        return minimapRooms;
    }
}
