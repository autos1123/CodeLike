using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private int ID;

    [SerializeField] private string interactionPrompt = "[F] 대화하기";
    [SerializeField] private Transform promptPivot;

    public string InteractionPrompt => interactionPrompt;

    public Transform PromptPivot => promptPivot;

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
