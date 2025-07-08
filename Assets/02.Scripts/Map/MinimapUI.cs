using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI:MonoBehaviour
{
    public RectTransform minimapPanel;
    public GameObject roomIconPrefab;
    public float roomSpacing = 50f; // 방 간격

    public void GenerateMinimap(List<MinimapRoomData> roomDataList)
    {
        // 기존 미니맵 아이콘 제거
        foreach(Transform child in minimapPanel)
            Destroy(child.gameObject);

        // 새로운 미니맵 아이콘 생성
        foreach(var room in roomDataList)
        {
            var icon = Instantiate(roomIconPrefab, minimapPanel);
            var rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(room.worldPosition.x * roomSpacing, room.worldPosition.y * roomSpacing);

            var img = icon.GetComponent<Image>();
            img.color = GetColorByRoomType(room.type);
        }
    }

    private Color GetColorByRoomType(RoomType type)
    {
        return type switch
        {
            RoomType.Start => Color.green,
            RoomType.Boss => Color.red,
            RoomType.Normal => Color.gray,
            _ => Color.white
        };
    }
}
