using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherStageManager : StageManager
{
    [SerializeField] private Room otherRoomPrefab;
    public bool IsTutorialStageReady { get; private set; } = false;
    protected override bool Persistent => false; 
    
    public override void LoadStage()
    {
        ClearStage();

        // 튜토리얼 방 하나만 생성
        if (otherRoomPrefab != null)
        {
            GameObject tutorialRoomGO = Instantiate(otherRoomPrefab.gameObject);
            Room tutorialRoom = tutorialRoomGO.GetComponent<Room>();
            tutorialRoom.Initialize(tutorialRoom.GetHashCode(), new Vector2Int(0, 0), RoomType.Start);

            if (tutorialRoom != null)
            {
                currentStage = new StageData(-1);
                currentStage.RegisterRoom(tutorialRoom, 1, 1);
                GameManager.Instance.Player.transform.position = currentStage.playerSpawnPoint;
            }
            else
            {
                Debug.LogError("할당된 튜토리얼 방 프리팹에 Room 컴포넌트가 없습니다.");
            }
            IsTutorialStageReady = true; 
        }
        else
        {
            Debug.LogError("튜토리얼 방 프리팹이 할당되지 않았습니다!");
        }
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
