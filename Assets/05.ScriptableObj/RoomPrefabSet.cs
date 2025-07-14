using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/RoomPrefabSet")]
public class RoomPrefabSet : ScriptableObject
{
    public GameObject startRoom;
    public GameObject bossRoom;

    [SerializeField]
    public List<GameObject> normalRooms;

    public GameObject shopRoom;

    public GameObject GetRandomPrefab(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startRoom,
            RoomType.Boss => bossRoom,
            RoomType.Normal => normalRooms[Random.Range(0, normalRooms.Count)],
            RoomType.Shop => shopRoom,
            _ => normalRooms[0],
            
        };
    }
}
