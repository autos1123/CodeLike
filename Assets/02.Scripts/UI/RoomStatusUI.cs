using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomText;
    private string currentStageName;
    private void Start()
    {
        StartCoroutine(SubscribeToStageManager());
    }

    private IEnumerator SubscribeToStageManager()
    {
        yield return new WaitUntil(() => StageManager.HasInstance && StageManager.Instance.CurrentStage != null);
        
        currentStageName = GetStageName(StageManager.Instance.CurrentStage.stageID);

        StageManager.Instance.CurrentStage.OnCurrentRoomChanged += UpdateRoomStatus;
        
        UpdateRoomStatus(StageManager.Instance.CurrentStage.CurrentRoom);
    }

    private void OnDestroy()
    {
        if (StageManager.HasInstance && StageManager.Instance.CurrentStage != null)
        {
            StageManager.Instance.CurrentStage.OnCurrentRoomChanged -= UpdateRoomStatus;
        }
    }

    private void UpdateRoomStatus(Room room)
    {
        string roomStatus = "";
        
        switch(room.Type)
        {
            case RoomType.Start:
                roomStatus = "시작 지점";
                break;
            case RoomType.End:
                roomStatus = "끝";
                break;
            case RoomType.Boss:
                roomStatus = "보스 방";
                break;
            case RoomType.Shop:
                roomStatus = "상점";
                break;
            case RoomType.Event:
                roomStatus = "이벤트 방";
                break;
            case RoomType.Normal:
            default:
                roomStatus = $"{room.Id}"; // 일반 방은 방 번호만 표시
                break;
        }
        
        SetRoomText($"{currentStageName}\n{roomStatus}");
    }
    private string GetStageName(int stageIndex)
    {
        return stageIndex switch
        {
            0 => "얼어붙은 폐허 : 초입",
            1 => "얼어붙은 폐허 : 길목",
            2 => "얼어붙은 폐허 : 전사의 쉼터",
            _ => $"스테이지 {stageIndex + 1}"
        };
    }
    public void SetRoomText(string text)
    {
        if (roomText != null)
        {
            roomText.text = text;
        }
    }
}
