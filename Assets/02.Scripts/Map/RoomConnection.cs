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
    public int FromRoomID { get; private set; }
    public int ToRoomID { get; private set; }
    public HorizontalDirection Direction { get; private set; }

    public RoomConnection(int fromRoomID, int toRoomID, HorizontalDirection direction)
    {
        FromRoomID = fromRoomID;
        ToRoomID = toRoomID;
        Direction = direction;
    }
}
