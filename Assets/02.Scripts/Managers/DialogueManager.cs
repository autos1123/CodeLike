using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera npcCamera;
    [SerializeField] public RenderTexture playerTexture;
    [SerializeField] public RenderTexture npcTexture;

    public void Start()
    {
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

    void SetCamera(Camera camera , Transform target)
    {
        camera.transform.SetParent(target, true);
        camera.transform.position = target.position + target.forward * 5f;
        camera.transform.LookAt(target); // 타겟 바라보게 설정
        camera.transform.position = camera.transform.position + Vector3.up;
        camera.gameObject.SetActive(true);
    }

    void ResetCamera(Camera camera)
    {
        camera.transform.SetParent(transform);
        camera.transform.gameObject.SetActive(false);
    }
}
