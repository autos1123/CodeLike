using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Portal:MonoBehaviour,IInteractable
{
    [SerializeField] private string interactionPrompt = "[F] 문열기";
    [SerializeField] private Transform promptPivot;
    [SerializeField] private Room room;
    [SerializeField] private MeshRenderer rendererMesh;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private Direction exitDirection;
    [SerializeField] private float offsetDistance = 3f;
    [SerializeField] private float cooldownDuration = 1f;
    [SerializeField] private Room destinationRoom;

    private static readonly Dictionary<GameObject, float> lastTeleportTimes = new();

    public string InteractionPrompt => interactionPrompt;
    public Transform PromptPivot => promptPivot;

    public Transform DestinationPoint
    {
        get => destinationPoint;
        set => destinationPoint = value;
    }
    public Direction ExitDirection
    {
        get => exitDirection;
        set => exitDirection = value;
    }
    public float OffsetDistance => offsetDistance;
    public float CooldownDuration => cooldownDuration;
    public Room DestinationRoom
    {
        get => destinationRoom;
        set => destinationRoom = value;
    }

    /// <summary>
    ///  초기화: Room 이벤트 등록 및 비활성화 처리
    /// </summary>
    private void Start()
    {
        room = GetComponentInParent<Room>();
        rendererMesh = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        rendererMesh.enabled = false;
        boxCollider.enabled = false;
        room.onRoomClear += onPotalActivated;
    }
    /// <summary>
    /// 방향에 따른 벡터 반환
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
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
    /// <summary>
    /// 방 클리어시 포탈 활성화
    /// </summary>
    public void onPotalActivated()
    {
        rendererMesh.enabled = true;
        boxCollider.enabled = true;
    }
    /// <summary>
    /// 방 이벤트 제거
    /// </summary>
    public void OnEnable()
    {
        if(room == null) return;
        room.onRoomClear -= onPotal;
    }
    /// <summary>
    /// 플레이어와의 상호작용으로 포탈 이동
    /// </summary>
    /// <param name="other"></param>
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

        Rigidbody rb = other.GetComponent<Rigidbody>();
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
    /// <summary>
    /// 인터랙트 기능 여부 항상 true
    /// </summary>
    /// <param name="interactor"></param>
    /// <returns></returns>
    public bool CanInteract(GameObject interactor)
    {
        return true;
    }
}
