using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItemBox : MonoBehaviour
{
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

        DialogueManager.Instance.onDialogue(interactor.transform, this.transform);
        // TODO : 대화 시작 로직 추가
    }
}
