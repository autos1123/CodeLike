using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData 
{
    public int stageID;
    public List<Room> rooms = new();
    public List<RoomConnection> connections = new();
    public Dictionary<int, Room> roomMap = new();

    public Vector3 playerSpawnPoint;
    public Room startRoom => rooms.Find(r => r.Type == RoomType.Start);
    public Room bossRoom => rooms.Find(r => r.Type == RoomType.Boss);  

    public void RegisterRoom(Room room)
    {
        if (!roomMap.ContainsKey(room.Id)) 
            roomMap.Add(room.Id, room); 
    }
}
public static class MinimapBuilder
{
    public static List<MinimapRoomData> BuildFromStage(StageData stageData, List<RoomConnection> connections)
    {
        var minimapRooms = new List<MinimapRoomData>();

        foreach(var room in stageData.rooms)
        {
            var data = new MinimapRoomData
            {
                roomID = room.Id,
                worldPosition = new Vector3(room.GridPosition.x, room.GridPosition.y, 0f),
                type = room.Type,
                connectedDirections = new List<Direction>()
            };

            foreach(var conn in connections)
            {

            }
        }
        return minimapRooms;
    }
}
