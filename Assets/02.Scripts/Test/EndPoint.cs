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
        room.onRoomClear += onPotal;
    }
    private Vector3 GetDirectionVector(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => Vector3.zero
        };
    }

    public void onPotal()
    {
        rendererMesh.enabled = true;
        boxCollider.enabled = true;
    }

    public void OnEnable()
    {
        if(room == null) return;
        room.onRoomClear -= onPotal;
    }
    public void Interact(GameObject other)
    {

        Debug.Log("끝");
        return;

        //UIManager.Instance.ShowUI<GameOver>() 맵 클리어 ui 생기면 추가
    }

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }
}
