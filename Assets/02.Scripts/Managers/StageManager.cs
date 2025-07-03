using UnityEngine;

public class StageManager : MonoBehaviour
{
    public ProceduralStageGenerator generator;
    public StageData currentStage;

    public int seed = 0;
    public int stageID = 0;

    public void LoadStage(int seed)
    {
        int randomSeed = Random.Range(0, int.MaxValue);
        int randomRoomCount = Random.Range(6, 11);

        generator.roomCount = randomRoomCount;
        ClearStage();

        currentStage = new StageData();
        currentStage.stageID = stageID++;

        var generatedRooms = generator.Generate(randomSeed);

        foreach (var room in generatedRooms)
        {
            currentStage.rooms.Add(room);
            currentStage.RegisterRoom(room);

            if(room.Type == RoomType.Start)
                currentStage.playerSpawnPoint = room.transform.position;   
        }
    }

    void Start()
    {
        LoadStage(0);
        // generatedRooms를 StageData 등으로 저장하는 로직도 필요
    }

    public void RestartStage() => LoadStage(seed);

    public void ClearStage()
    {
        foreach (var room in FindObjectsOfType<Room>())
            Destroy(room.gameObject);
            currentStage = null;
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

