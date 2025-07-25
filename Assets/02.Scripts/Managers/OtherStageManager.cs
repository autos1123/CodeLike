using UnityEngine;

public class OtherStageManager : StageManager
{
    [SerializeField] private Room otherRoomPrefab;
    public bool IsStageReady { get; private set; } = false;
    protected override bool Persistent => false; 
    public ViewCameraController viewCameraController; 
    public override void LoadStage()
    {
        ClearStage();

        // 튜토리얼 방 하나만 생성
        if (otherRoomPrefab != null)
        {
            GameObject otherRoomGO = Instantiate(otherRoomPrefab.gameObject);
            Room otherRoom = otherRoomGO.GetComponent<Room>();
            otherRoom.Initialize(otherRoom.GetHashCode(), new Vector2Int(0, 0), RoomType.Start);

            if (otherRoom != null)
            {
                currentStage = new StageData(-1);
                currentStage.RegisterRoom(otherRoom, 1, 1);
                GameManager.Instance.Player.transform.position = currentStage.playerSpawnPoint;
            }
            else
            {
                Debug.LogError("할당된 튜토리얼 방 프리팹에 Room 컴포넌트가 없습니다.");
            }
            IsStageReady = true; 
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
        
        if (viewCameraController != null)
        {
            Debug.Log("카메라 2D 세팅 완료");
            viewCameraController.SetCameraInstant(ViewModeType.View2D);
        }
        else
        {
            Debug.LogWarning("[LobbyStageManager] ViewCameraController not found. Cannot force 2D view instantly.");
        }
    }
}
