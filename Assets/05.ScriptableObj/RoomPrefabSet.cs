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

    private int n_prevRoomIdx = -1; // 직전에 생성된 일방 룸의 인덱스

    public GameObject GetRandomPrefab(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startRoom,
            RoomType.End => endRoom,
            RoomType.Normal => GetRandomNormalRoom(),
            RoomType.Shop => shopRoom,
            RoomType.Boss => bossRoom,
            _ => normalRooms[0],
            
        };
    }

    private GameObject GetRandomNormalRoom()
    {
        int idx;

        if(n_prevRoomIdx == -1)
        {
            idx = Random.Range(0, normalRooms.Count);
            n_prevRoomIdx = idx;
            return normalRooms[idx];
        }

        do
        {
            idx = Random.Range(0, normalRooms.Count);
        } while(idx == n_prevRoomIdx); // 직전 룸과 같은 인덱스는 제외
        n_prevRoomIdx = idx;
        return normalRooms[idx];
    }
}
