using System.Collections;
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
