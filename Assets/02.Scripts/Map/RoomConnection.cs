public enum Direction
{
    Up = -1,
    Down = 1,
    Left = -2,
    Right = 2
}

public class RoomConnection 
{
    public int FromRoomID { get; private set; }
    public int ToRoomID { get; private set; }
    public Direction Direction { get; private set; }

    public RoomConnection(int fromRoomID, int toRoomID, Direction direction)
    {
        FromRoomID = fromRoomID;
        ToRoomID = toRoomID;
        Direction = direction;
    }

}
