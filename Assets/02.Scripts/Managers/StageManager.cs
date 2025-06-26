using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public ProceduralStageGenerator generator;
    public StageData currentStage;

    public int seed = 0;
    public int stageID = 0;

    public void LoadStage(int seed)
    {
        ClearStage();

        currentStage = new StageData();
        currentStage.stageID = stageID++;

        var generatedRooms = generator.Generate(seed);
        foreach (var room in generatedRooms)
        {
            currentStage.rooms.Add(room);
            currentStage.RegisterRoom(room);

            if(room.Type == RoomType.Start)
                currentStage.playerSpawnPoint = room.transform.position;   
        }
    }

    public void RestartStage() => LoadStage(seed);

    public void ClearStage()
    {
        foreach (var room in FindObjectsOfType<Room>())
            Destroy(room.gameObject);
            currentStage = null;
    }
}
