using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "[F] 스테이지 클리어";
    [SerializeField] private Transform promptPivot;
    [SerializeField] Room room;
    [SerializeField] MeshRenderer rendererMesh;
    [SerializeField] BoxCollider boxCollider;

    private static readonly Dictionary<GameObject, float> lastTeleportTimes = new();

    public string InteractionPrompt => interactionPrompt;

    public Transform PromptPivot => promptPivot;

    private void Start()
    {
        room = GetComponentInParent<Room>();
        rendererMesh = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        rendererMesh.enabled = false;
        boxCollider.enabled = false;
        room.onRoomClear += on;
    }

    public void on()
    {
        rendererMesh.enabled = true;
        boxCollider.enabled = true;
    }

    public void OnEnable()
    {
        if(room == null) return;
        room.onRoomClear -= on;
    }
    public void Interact(GameObject other)
    {
        StageManager.Instance.LoadStage();
        return;

        //UIManager.Instance.ShowUI<GameOver>() 맵 클리어 ui 생기면 추가
    }

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }
}
