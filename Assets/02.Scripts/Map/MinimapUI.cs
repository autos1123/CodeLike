using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI: UIBase
{
    public RectTransform minimapPanel;
    public GameObject roomIconPrefab;
    public float roomSpacing = 5f; // 방 간격

    Dictionary<(int x, int y), GameObject> Rooms = new();

    PlayerController player;
    public override string UIName => "MinimapUI"; // 중요!

    public Transform minimapRoot; // 빈 오브젝트, 미니맵 아이콘 모음용
    public override void Open()
    {
        base.Open();
        if(player == null) player = GameManager.Instance.Player.GetComponent<PlayerController>();

        BuildMinimap(StageManager.Instance.CurrentStage);        
        minimapRoot.GetComponent<RectTransform>().anchoredPosition = -1 * new Vector2( player.CurrentRoom.x* 250, player.CurrentRoom.y * 250);

        Rooms[(player.CurrentRoom.x, player.CurrentRoom.y)].transform.GetComponent<Outline>().enabled = true;
    }
    public override void Close()
    {
        Rooms[(player.CurrentRoom.x, player.CurrentRoom.y)].transform.GetComponent<Outline>().enabled = false;
        base.Close();
    }

    
    public void GenerateMinimap(List<MinimapRoomData> roomDataList)
    {
        foreach(Transform child in minimapRoot)
            Destroy(child.gameObject); // 또는 Pool에서 회수

        foreach(var room in roomDataList)
        {
            var icon = Instantiate(roomIconPrefab, minimapRoot);
            var rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(
                room.worldPosition.x * roomSpacing,
                room.worldPosition.y * roomSpacing
            );

            Rooms[((int)(room.worldPosition.x/250),(int)(room.worldPosition.y / 250))] = icon;

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

    public void BuildMinimap(StageData stageData)
    {
        if(UIManager.Instance.TryGetUI<MinimapUI>(out var minimap))
        {
            var minimapData = MinimapBuilder.BuildFromStage(stageData, stageData.connections);
            minimap.GenerateMinimap(minimapData);
        }
        else
        {
            Debug.LogError(" MinimapUI를 찾을 수 없습니다.");
        }
    }
}
