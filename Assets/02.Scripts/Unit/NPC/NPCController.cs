using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "[E] 대화하기";

    public string InteractionPrompt => interactionPrompt;

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }

    public void Interact(GameObject interactor)
    {
        UIManager.Instance.ShowUI<DialogueBoard>();
        // TODO : 대화 시작 로직 추가
    }
}
