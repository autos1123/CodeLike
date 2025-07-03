using UnityEngine;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera npcCamera;
    RenderTexture playerTexture;
    RenderTexture npcTexture;

    public void Start()
    {
        playerCamera = transform.GetChild(0).GetComponent<Camera>();
        npcCamera = transform.GetChild(1).GetComponent<Camera>();
        playerCamera.gameObject.SetActive(false);
        npcCamera.gameObject.SetActive(false);
    }
    public void onDialogue(string npc)
    {
        //Todo npc정보를 기반으로 출력할 텍스트 정리
        //npc앞과 플레이어 앞에 가메라 위치 고정
    }

    public void offDialogue()
    {
        playerCamera.targetTexture = null;
        //todo 이미지에 있는 택스처도 꺼준다.
    }
}
