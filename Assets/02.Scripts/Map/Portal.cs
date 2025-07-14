using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Portal:MonoBehaviour,IInteractable
{
    [SerializeField] private string interactionPrompt = "[F] 문열기";
    [SerializeField] private Transform promptPivot;
    [SerializeField] Room room;
    [SerializeField] MeshRenderer rendererMesh;
    [SerializeField] BoxCollider boxCollider;
    public Transform destinationPoint;
    public Direction exitDirection;
    public float offsetDistance = 3f;
    public float cooldownDuration = 1f;
    public Room destinationRoom;

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

        if(!room.isClearRoom)
        {
            return;
        }
        if(!other.CompareTag("Player") || destinationPoint == null)
            return;

        float currentTime = Time.time;
        if(lastTeleportTimes.TryGetValue(other.gameObject, out float lastTime))
        {
            if(currentTime - lastTime < cooldownDuration)
                return;
        }

        // 다음 방 활성화
        destinationRoom.gameObject.SetActive(true);

        // 위치 이동: 연결 방향 기반으로 오프셋 적용

        var rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        Vector3 offset = GetDirectionVector(exitDirection) * offsetDistance;
        other.transform.position = destinationPoint.position + offset;

        lastTeleportTimes[other.gameObject] = currentTime;

        // 반대편 포탈도 쿨타임 동기화
        Portal destPortal = destinationPoint.GetComponentInParent<Portal>();
        if(destPortal != null)
        {
            lastTeleportTimes[other.gameObject] = currentTime;
        }

        room.gameObject.SetActive(false); // 현재 방 비활성화
    }

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }
}
