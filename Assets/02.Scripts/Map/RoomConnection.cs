using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HorizontalDirection
{
    Left,
    Right
}

public class RoomConnection 
{
    public int FromRoomID { get; set; }
    public int ToRoomID { get; set; }
    public HorizontalDirection Direction { get; set; }

    public RoomConnection(int fromRoomID, int toRoomID, HorizontalDirection direction)
    {
        FromRoomID = fromRoomID;
        ToRoomID = toRoomID;
        Direction = direction;
    }
}
