using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/RoomPrefabSet")]
public class RoomPrefabSet : ScriptableObject
{
    public GameObject startRoom;
    public GameObject endRoom;

    [SerializeField]
    public List<GameObject> normalRooms;

    public GameObject shopRoom;
    public GameObject bossRoom;

    public GameObject GetRandomPrefab(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startRoom,
            RoomType.End => endRoom,
            RoomType.Normal => normalRooms[Random.Range(0, normalRooms.Count)],
            RoomType.Shop => shopRoom,
            RoomType.Boss => bossRoom,
            _ => normalRooms[0],
            
        };
    }
}
