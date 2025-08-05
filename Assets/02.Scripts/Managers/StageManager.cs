using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager:MonoSingleton<StageManager>
{
    // 변수는 가급적 private으로 선언
    // 만약 인스펙터 노출이 필요하면 [SerializeField] 사용

    protected override bool Persistent => false;

    // private으로 변경 후 외부 접근을 위해 프로퍼티 추가 필요
    [SerializeField] protected ProceduralStageGenerator generator;
    protected StageData currentStage;

    public ProceduralStageGenerator Generator => generator; // 외부에서 접근할 수 있도록 프로퍼티로 노출
    public StageData CurrentStage => currentStage;

    protected int stageID = 0; // private으로

    public event Action ChangeStage;

    protected Coroutine waitUntilCoroutine;

    //스테이지 마다 생성할 맵의 수
    [SerializeField] protected int[] stageMapCountData = {5, 6, 7, 8, 9, 10 };    

    //그리드 반경 및 높이
    public int gridWidth { get; protected set; } = 10;
    public int gridHeight { get; protected set; } = 10;

    public int[] StageMapCountData => stageMapCountData;

    public ViewCameraController viewCameraController; 
    
    protected bool titleShown = false;

    [SerializeField] protected string bgmKey;              // 인스펙터에서 지정 (예: "TutorialBGM")
    [SerializeField] protected Material skyboxMaterial;    // 인스펙터에서 지정
    public virtual void LoadStage()
    {
        titleShown = false;
        
        if (stageID >= stageMapCountData.Length)
        {
            SceneManager.LoadScene("EndingScene");
            return;
        }
        
        ApplyStageEnvironment();
        int randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
        ClearStage();
        
        int displayStageID = stageID;
        int roomCountBase = stageMapCountData[stageID];

        currentStage = generator.Generate(randomSeed, roomCountBase, stageID);

        if(currentStage.roomMap.Count != 0)
        {
            GameManager.Instance.Player.transform.position = currentStage.playerSpawnPoint;
            if (viewCameraController != null && ViewManager.HasInstance)
            {
                Debug.Log($"[StageManager] 씬 로드 시 현재 뷰 모드 유지: {ViewManager.Instance.CurrentViewMode}");
                // ViewCameraController에게 ViewManager가 현재 기억하는 뷰 모드를 전달하여 초기화
                viewCameraController.InitCameraForStage(ViewManager.Instance.CurrentViewMode); 
            }
            else if (viewCameraController == null)
            {
                Debug.LogWarning("[StageManager] ViewCameraController가 할당되지 않았습니다! 카메라 뷰를 설정할 수 없습니다.");
                viewCameraController = Camera.main.GetComponent<ViewCameraController>();
            }
            else 
            {
                Debug.LogWarning("[StageManager] ViewManager가 아직 초기화되지 않았습니다. 기본 뷰 모드(2D)로 설정합니다.");
                viewCameraController.InitCameraForStage(ViewModeType.View2D); 
            }
        }
        else
        {
            Debug.LogWarning("시작 방이 존재하지 않습니다.");
        }

        if(waitUntilCoroutine != null)
            StopCoroutine(waitUntilCoroutine);

        waitUntilCoroutine = StartCoroutine(WaitUntilCreateUI());
        
        TryShowMapTitle(displayStageID);
        
        ChangeStage?.Invoke();

        stageID++;
    }

    void Start()
    {
        LoadStage();
        //ApplyStageEnvironment();
        // 나중에 저장 로직 추가 가능
    }

    public void ClearStage()
    {
        // allRooms 리스트에 Room이 존재하는 경우
        // 모든 방을 비활성화

        if(currentStage == null)
            return;

        foreach(KeyValuePair<int, Room> room in currentStage.roomMap)
        {
            room.Value.gameObject.SetActive(false);
        }

        currentStage = null;
    }


    IEnumerator WaitUntilCreateUI()
    {
        yield return new WaitUntil(() => UIManager.Instance.GetUI<MinimapUI>() != null);
        UIManager.Instance.GetUI<MinimapUI>().BuildMinimap();
    }
    
    private void TryShowMapTitle(int stageIndex)
    {
        if (titleShown) return;

        void Show()
        {
            if (UIManager.Instance.TryGetUI<MapTitleUI>(out var mapTitleUI))
            {
                UIManager.Instance.ShowUI<MapTitleUI>();

                //맵 타이틀 텍스트 설정
                string titleText = stageIndex switch
                {
                    0 => "얼어붙은 폐허 : 초입",
                    1 => "얼어붙은 폐허 : 길목",
                    2 => "얼어붙은 폐허 : 전사의 쉼터",
                    _ => $"스테이지 {stageIndex + 1}"
                };

                mapTitleUI.ShowTitle(titleText);
                titleShown = true;
            }
        }

        if (UIManager.Instance.IsUILoaded())
            Show();
        else
            UIManager.Instance.OnAllUIReady(() => Show());
    }

    public void ReLoadStage()
    {
        stageID--;
        LoadStage();
    }

    protected void ApplyStageEnvironment()
    {
        // BGM 재생
        if(SoundManager.HasInstance && !string.IsNullOrEmpty(bgmKey))
            SoundManager.Instance.PlayBGM(null, bgmKey);
        
        // 스카이박스 변경
        if(skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
    }
    // ===== 미니맵 시스템 전달용 데이터 정리 =====
    // 이 구문은 현재 생성된 모든 Room 정보를 바탕으로
    // 미니맵 UI 제작자가 사용할 수 있도록 필요한 데이터를 추출하는 파트입니다.
    //
    // MinimapRoomData에는 다음 정보가 포함됩니다:
    // - roomID: 고유 ID로, 각 방을 식별하는 기준입니다.
    // - worldPosition: 월드 좌표계 기준 방의 중심 위치입니다. 미니맵 아이콘의 위치로 사용됩니다.
    // - type: 방의 타입 (Start, Normal, Boss 등). 미니맵에서 아이콘 색상 등으로 구분 가능합니다.
    // - connectedDirections: 이 방이 어느 방향으로 다른 방과 연결되어 있는지 나타내는 방향 목록입니다.
    //                        (예: Up, Down, Left, Right)
    // 
    // 이 리스트를 미니맵 시스템에 넘겨주면, 각 방을 미니맵에 그릴 수 있고
    //    연결선까지 자동 생성할 수 있습니다.
    //
    // 사용 예:
    // List<MinimapRoomData> minimapRoomDataList = new();
    // foreach (var room in currentStage.rooms)
    //     minimapRoomDataList.Add(room.GetMinimapData());
    //
    // 이 minimapRoomDataList를 미니맵 담당 시스템으로 넘겨주세요.
}

