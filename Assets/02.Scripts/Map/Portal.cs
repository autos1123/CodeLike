using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Portal:MonoBehaviour,IInteractable
{
    [SerializeField] private string interactionPrompt = "[F] 문열기";
    [SerializeField] private Transform promptPivot;
    private Room room;
    [SerializeField] private ParticleSystem nextRoomParticle;
    [SerializeField] private ParticleSystem prevRoomParticle;
    [SerializeField] private GameObject direction_right;
    [SerializeField] private GameObject direction_left;
    private BoxCollider boxCollider;
    private Direction exitDirection;
    [SerializeField] private float offsetDistance = 3f;
    [SerializeField] private float cooldownDuration = 1f;
    private Room destinationRoom;

    private static float lastTeleportTimes = 0;
    private ScreenFader screenFader;
    private bool isTeleporting = false;
    
    public string InteractionPrompt => interactionPrompt;
    public Transform PromptPivot => promptPivot;
    public Direction ExitDirection => exitDirection;
    public float OffsetDistance => offsetDistance;
    public float CooldownDuration => cooldownDuration;
    public Room DestinationRoom => destinationRoom;
    public GameObject DirectionRight => direction_right;
    public GameObject DirectionLeft => direction_left;

    /// <summary>
    ///  초기화: Room 이벤트 등록 및 비활성화 처리
    /// </summary>
    private void Start()
    {
        room = GetComponentInParent<Room>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;

        if(screenFader == null)
        {
            screenFader = FindObjectOfType<ScreenFader>();
        }
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
        if(destinationRoom == null)
            nextRoomParticle.Play();

        if(destinationRoom.isClearRoom)
        {
            prevRoomParticle.Play();
        }
        else
        {
            nextRoomParticle.Play();
        }
        boxCollider.enabled = true;
    }

    /// <summary>
    /// 플레이어와의 상호작용으로 포탈 이동
    /// </summary>
    /// <param name="other"></param>
    public void Interact(GameObject other)
    {
        if(isTeleporting) return;
        
        StartCoroutine(Teleport(other));
    }

    private IEnumerator Teleport(GameObject other)
    {
        isTeleporting = true;

        if(screenFader != null)
        {
            yield return screenFader.FadeOut();
        }
        
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        if(destinationRoom == null)
        {
            StageManager.Instance.LoadStage();
            isTeleporting = false;
            yield break;
        }
        var pos = destinationRoom.GetEntranceAnchor((Direction)((int)exitDirection * -1)).position;

        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D) pos.z = 0;

        other.transform.position = pos + (Vector3.up * 0.2f);

        lastTeleportTimes = Time.time;

        StageManager.Instance.SetCurrentRoom(destinationRoom);
        
        isTeleporting = false;
        
    }
    /// <summary>
    /// 인터랙트 기능 여부 항상 true
    /// </summary>
    /// <param name="interactor"></param>
    /// <returns></returns>
    public bool CanInteract(GameObject interactor)
    {
        if(room == null) return false;
        return room.isClearRoom && Time.time - lastTeleportTimes > cooldownDuration;
    }
}
