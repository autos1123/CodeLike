using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData 
{
    public int stageID { get; private set; } // 현재 초기화 안됨
    public List<RoomConnection> connections { get; private set; } = new();
    public Dictionary<int, Room> roomMap { get; private set; } = new();
    public Vector3 playerSpawnPoint { get; private set; }
    //public Room CurrentRoom { get; private set; }
    public event Action<Room> OnCurrentRoomChanged;
    private Room _currentRoom;
    public Room CurrentRoom 
    {
        get => _currentRoom;
        private set
        {
            // 같은 방이면 불필요한 업데이트를 방지
            if (_currentRoom == value) return;
            _currentRoom = value;
            // 현재 방이 변경될 때마다 이벤트를 호출
            OnCurrentRoomChanged?.Invoke(_currentRoom);
        }
    }

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
            CurrentRoom = room;
            playerSpawnPoint = room.GetPlayerSpawnPoint();
        }
        else
        {
            room.gameObject.SetActive(false);
        }
    }

    public void SetCurrentRoom(Room room)
    {
        CurrentRoom = room;
        UIManager.Instance.GetUI<MinimapUI>().UpdateCurrentRoomInfo();
    }
}
