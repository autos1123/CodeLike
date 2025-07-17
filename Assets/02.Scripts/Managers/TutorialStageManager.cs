using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStageManager : StageManager
{
    [SerializeField] private Room tutorialRoomPrefab;

    protected override bool Persistent => false; 
    
    public override void LoadStage()
    {
        ClearStage();

        // 튜토리얼 방 하나만 생성
        if (tutorialRoomPrefab != null)
        {
            GameObject tutorialRoomGO = Instantiate(tutorialRoomPrefab.gameObject);
            Room tutorialRoom = tutorialRoomGO.GetComponent<Room>();
            if (tutorialRoom != null)
            {
                allRooms = new List<Room> { tutorialRoom };
                
                currentStage = new StageData(); 
                currentStage.InitializeGrid(1, 1);
                
                currentStage.roomMap = new Dictionary<int, Room>();
                currentStage.roomMap.Add(tutorialRoom.GetHashCode(), tutorialRoom); // 방 고유 ID 대신 해시코드를 임시 사용
                currentStage.SetStartRoom(tutorialRoom); 
                currentStage.stageID = 0; 

                currentStage.playerSpawnPoint = tutorialRoom.GetPlayerSpawnPoint();
                GameManager.Instance.Player.transform.position = currentStage.playerSpawnPoint;

                Debug.Log("튜토리얼 스테이지 로드 완료: 단일 방 생성");
            }
            else
            {
                Debug.LogError("할당된 튜토리얼 방 프리팹에 Room 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("튜토리얼 방 프리팹이 할당되지 않았습니다!");
        }

        OnStageChanged();
    }
    
    protected override void Awake()
    {
        base.Awake(); 
    }

    void Start()
    {
        LoadStage();
    }
}
