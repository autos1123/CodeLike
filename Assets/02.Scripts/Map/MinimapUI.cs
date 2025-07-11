using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI: UIBase
{
    public RectTransform minimapPanel;
    public GameObject roomIconPrefab;
    public float roomSpacing = 5f; // 방 간격

    Transform player;
    public override string UIName => "MinimapUI"; // 중요!

    public Transform minimapRoot; // 빈 오브젝트, 미니맵 아이콘 모음용
    public override void Open()
    {
        base.Open();
        if(player == null) player = GameManager.Instance.Player.transform;

        BuildMinimap(StageManager.Instance.currentStage);        
        minimapRoot.GetComponent<RectTransform>().anchoredPosition = -1 * new Vector2( (int)(player.localPosition.x/250+0.5)* 200, (int)(player.localPosition.y / 250+ 0.5) * 200);
    }
    public override void Close()
    {

        base.Close();
    }

    
    public void GenerateMinimap(List<MinimapRoomData> roomDataList)
    {
        Debug.Log($" 미니맵 생성 시작 - 방 수: {roomDataList.Count}");

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
            Debug.Log($" Minimap 생성 완료: {minimapData.Count}개 방");
        }
        else
        {
            Debug.LogError(" MinimapUI를 찾을 수 없습니다.");
        }
    }
}
