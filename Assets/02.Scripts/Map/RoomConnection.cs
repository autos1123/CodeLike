public enum Direction
{
    Up,
    Down,
    Left,
    Right
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
