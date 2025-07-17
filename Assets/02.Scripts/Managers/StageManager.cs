using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager:MonoSingleton<StageManager>
{
    // 변수는 가급적 private으로 선언
    // 만약 인스펙터 노출이 필요하면 [SerializeField] 사용

    protected override bool Persistent => false;

    DestinyManager destinyManager;

    // private으로 변경 후 외부 접근을 위해 프로퍼티 추가 필요
    [SerializeField] private ProceduralStageGenerator generator;
    [SerializeField] private StageData currentStage;

    public ProceduralStageGenerator Generator => generator; // 외부에서 접근할 수 있도록 프로퍼티로 노출
    public StageData CurrentStage => currentStage;

    private int stageID = 0; // private으로

    public event Action ChangeStage;

    private Coroutine waitUntilCoroutine;

    //스테이지 마다 생성할 맵의 수
    [SerializeField] private int[] stageMapCountData = {5, 6, 7, 8, 9, 10 };    

    //그리드 반경 및 높이
    public int gridWidth { get; private set; } = 10;
    public int gridHeight { get; private set; } = 10;

    public int[] StageMapCountData => stageMapCountData;

    private void OnEnable()
    {
        if(destinyManager == null)
            destinyManager = DestinyManager.Instance;

        destinyManager.onDestinyChange += HandleDestinyChange;
    }

    private void OnDisable()
    {
        destinyManager.onDestinyChange -= HandleDestinyChange;
    }

    public void LoadStage()
    {
        int randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
        ClearStage();

        int roomCountBase = stageMapCountData[stageID];

        currentStage = generator.Generate(randomSeed, roomCountBase, stageID);

        if(currentStage.roomMap.Count != 0)
        {
            GameManager.Instance.Player.transform.position = currentStage.playerSpawnPoint;
        }
        else
        {
            Debug.LogWarning("시작 방이 존재하지 않습니다.");
        }

        if(waitUntilCoroutine != null)
            StopCoroutine(waitUntilCoroutine);

        waitUntilCoroutine = StartCoroutine(WaitUntilCreateUI());
        ChangeStage?.Invoke();

        stageID++;
    }

    void Start()
    {
        LoadStage();        
        // 나중에 저장 로직 추가 가능
    }

    public void ClearStage()
    {
        // allRooms 리스트에 Room이 존재하는 경우
        // 모든 방을 비활성화

        foreach(var room in FindObjectsOfType<Room>())
            Destroy(room.gameObject); // Destroy 대신 오브젝트 비활성화

        currentStage = null;
    }

    void HandleDestinyChange(DestinyData data, int i)
    {
        DestinyEffectData positiveEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.PositiveEffectDataID);
        DestinyEffectData negativeEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.NegativeEffectDataID);

        if(positiveEffect.effectedTarget == EffectedTarget.Map)
        {
            stageMapCountData = stageMapCountData.Select(n => n + (int)positiveEffect.value * i).ToArray();
        }

        if(negativeEffect.effectedTarget == EffectedTarget.Map)
        {
            stageMapCountData = stageMapCountData.Select(n => n - (int)positiveEffect.value * i).ToArray();
        }

    }

    IEnumerator WaitUntilCreateUI()
    {
        yield return new WaitUntil(() => UIManager.Instance.GetUI<MinimapUI>() != null);
        UIManager.Instance.GetUI<MinimapUI>().BuildMinimap();
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

