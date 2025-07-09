using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRoomData //사용법 StageManager.cs 참조 
{
    public int roomID;
    public Vector3 worldPosition;
    public RoomType type;
    public List<Direction> connectedDirections;

    public bool isCurrent = false;
}


