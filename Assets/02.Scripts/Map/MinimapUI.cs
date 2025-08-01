using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI: UIBase
{
    [SerializeField] private GameObject roomIconPrefab;
    [SerializeField] private Vector2 roomIconSize = new Vector2(200, 100);
    [SerializeField] private float roomSpacing = 50;
    [SerializeField] private float minimapMargin = 50;

    private Dictionary<Vector2Int, RoomIcon> roomIcons = new();
    private List<RoomIcon> activeRoomImage = new();
    private RoomIcon currentRoomUI;

    PlayerController player;
    public override string UIName => this.GetType().Name;

    public RectTransform minimapRoot; // 빈 오브젝트, 미니맵 아이콘 모음용

    private void Awake()
    {
        int gridWidth = StageManager.Instance.gridWidth;
        int gridHeight = StageManager.Instance.gridHeight;

        float minimapWidth = (roomIconSize.x * gridWidth) + (roomSpacing * (gridWidth - 1)) + minimapMargin * 2;
        float minimapHeight = (roomIconSize.y * gridHeight) + (roomSpacing * (gridHeight - 1)) + minimapMargin * 2;

        minimapRoot.sizeDelta = new Vector2(minimapWidth, minimapHeight); // 초기 크기 설정
        GenerateMinimap(gridWidth, gridHeight);
    }

    public override void Open()
    {
        base.Open();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"MapUIOpen");
        if(player == null) player = GameManager.Instance.Player.GetComponent<PlayerController>();
        UpdateCurrentRoomInfo();
    }
    public override void Close()
    {
        base.Close();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"MapUIClose");
    }

    
    public void GenerateMinimap(int gridWidth, int gridHeight)
    {
        for(int i = 0; i < gridHeight; i++)
        {
            for(int j = 0; j < gridWidth; j++)
            {
                RoomIcon roomUI = Instantiate(roomIconPrefab, minimapRoot).GetComponent<RoomIcon>();
                roomUI.ChangeColor(Color.clear); // 초기에는 투명하게 설정
                roomIcons[new Vector2Int(j, i)] = roomUI;
            }
        }
    }

    private Color GetColorByRoomType(RoomType type)
    {
        return type switch
        {
            RoomType.Start => Color.green,
            RoomType.End => Color.red,
            RoomType.Normal => Color.gray,
            _ => Color.white
        };
    }

    public void BuildMinimap()
    {
        // 이전에 활성화된 미니맵 UI가 있는 경우 비활성화
        if(activeRoomImage.Count != 0)
        {
            for(int i = 0; i < activeRoomImage.Count; i++)
            {
                activeRoomImage[i].ResetUI();
            }
            activeRoomImage.Clear();
        }

        StageData stageData = StageManager.Instance.CurrentStage;

        foreach(KeyValuePair<int, Room> pair in stageData.roomMap)
        {
            Vector2Int index = pair.Value.GridPosition;

            roomIcons[index].ChangeColor(GetColorByRoomType(pair.Value.Type));
            activeRoomImage.Add(roomIcons[index]);

            for(int i = 0; i < pair.Value.Connections.Count; i++)
            {
                roomIcons[index].SetPortalUI(pair.Value.Connections[i].Direction);
            }
        }

        foreach(RoomConnection con in stageData.connections)
        {
            Vector2Int idx = stageData.roomMap[con.FromRoomID].GridPosition;
        }
    }

    public void UpdateCurrentRoomInfo()
    {
        StageData stageData = StageManager.Instance.CurrentStage;

        Vector2Int currentIndex = stageData.CurrentRoom.GridPosition;

        if(currentRoomUI != null)
        {
            currentRoomUI.SetOutLine(false);
            currentRoomUI = null;
        }
        currentRoomUI = roomIcons[currentIndex];
        currentRoomUI.SetOutLine(true);

        currentIndex += Vector2Int.one;

        Vector2 iconLenth;
        iconLenth.x = (currentIndex.x * roomIconSize.x) - (roomIconSize.x / 2);
        iconLenth.y = (currentIndex.y * roomIconSize.y) - (roomIconSize.y / 2);

        Vector2 spacingLenth;
        spacingLenth.x = roomSpacing * (currentIndex.x - 1);
        spacingLenth.y = roomSpacing * (currentIndex.y - 1);

        Vector2 minimapPos;
        minimapPos.x = (minimapMargin + iconLenth.x + spacingLenth.x) * -1;
        minimapPos.y = (minimapMargin + iconLenth.y + spacingLenth.y) * -1;

        minimapRoot.anchoredPosition = minimapPos;
    }
}
