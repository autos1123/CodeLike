using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Portal:MonoBehaviour,IInteractable
{
    [SerializeField] private string interactionPrompt = "[F] 문열기";
    [SerializeField] private Transform promptPivot;
    [SerializeField] private Room room;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Direction exitDirection;
    [SerializeField] private float offsetDistance = 3f;
    [SerializeField] private float cooldownDuration = 1f;
    [SerializeField] private Room destinationRoom;

    private static float lastTeleportTimes = 0;

    public string InteractionPrompt => interactionPrompt;
    public Transform PromptPivot => promptPivot;
    public Direction ExitDirection => exitDirection;
    public float OffsetDistance => offsetDistance;
    public float CooldownDuration => cooldownDuration;
    public Room DestinationRoom => destinationRoom;

    /// <summary>
    ///  초기화: Room 이벤트 등록 및 비활성화 처리
    /// </summary>
    private void Start()
    {
        room = GetComponentInParent<Room>();
        boxCollider = GetComponent<BoxCollider>();
        particle.Stop();
        boxCollider.enabled = false;
    }

    public void InitPortal(Direction exitDirection, Room destinationRoom)
    {
        this.exitDirection = exitDirection;
        this.destinationRoom = destinationRoom;
    }

    /// <summary>
    /// 방 클리어시 포탈 활성화
    /// </summary>
    public void OnPotalActivated()
    {
        particle.Play();
        boxCollider.enabled = true;
    }

    /// <summary>
    /// 플레이어와의 상호작용으로 포탈 이동
    /// </summary>
    /// <param name="other"></param>
    public void Interact(GameObject other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        if(destinationRoom == null)
        {
            StageManager.Instance.LoadStage();
            return;
        }

        // 다음 방 활성화
        destinationRoom.gameObject.SetActive(true);

        other.transform.position = destinationRoom.GetSponPos().position;

        lastTeleportTimes = Time.time;

        room.gameObject.SetActive(false); // 현재 방 비활성화

        StageManager.Instance.CurrentStage.SetCurrentRoom(destinationRoom);
    }
    /// <summary>
    /// 인터랙트 기능 여부 항상 true
    /// </summary>
    /// <param name="interactor"></param>
    /// <returns></returns>
    public bool CanInteract(GameObject interactor)
    {
        return room.isClearRoom && Time.time - lastTeleportTimes > cooldownDuration;
    }
}
