using UnityEngine;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera npcCamera;
    [SerializeField] public RenderTexture playerTexture;
    [SerializeField] public RenderTexture npcTexture;

    public void Start()
    {
        playerCamera = transform.GetChild(0).GetComponent<Camera>();
        npcCamera = transform.GetChild(1).GetComponent<Camera>();
        playerCamera.gameObject.SetActive(false);
        npcCamera.gameObject.SetActive(false);
    }
    public void onDialogue(Transform player , Transform npc)
    {
        //Todo npc정보를 기반으로 출력할 텍스트 정리
        playerCamera.targetTexture = playerTexture;
        npcCamera.targetTexture = npcTexture;

        SetCamera(playerCamera, player);
        SetCamera(npcCamera, npc);
    }

    public void offDialogue()
    {
        playerCamera.targetTexture = null;
        ResetCamera(playerCamera);
        ResetCamera(npcCamera);
    }

    void SetCamera(Camera camera , Transform transform)
    {
        
        camera.transform.position = transform.forward * 5;//죄표 조절 필요
        camera.transform.forward = transform.position;
        //camera.transform.position = transform.up * 2;
        camera.transform.parent = transform.transform;
        camera.transform.gameObject.SetActive(true);
    }

    void ResetCamera(Camera camera)
    {
        camera.transform.parent = this.transform;
        camera.transform.gameObject.SetActive(false);
    }
}
