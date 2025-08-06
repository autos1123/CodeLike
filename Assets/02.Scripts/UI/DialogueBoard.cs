using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoard : UIBase
{
    [SerializeField] TMP_Text Conversation;
    [SerializeField] RawImage playerRawImage;
    [SerializeField] RawImage npcRawImage;
    [SerializeField] Button beforButton;
    [SerializeField] Button nextButton;

    
    List<string> messages;
    public int messagesPointer = 0;
    private Tween typingTween;
    private bool isTyping = false;
    public override string UIName => this.GetType().Name;

    public void Init(List<string> strings)
    {
        messages = strings;
        messagesPointer = 0;
    }
    public override void Open()
    {
        base.Open();
        playerRawImage.texture = DialogueManager.Instance.playerTexture;
        npcRawImage.texture = DialogueManager.Instance.npcTexture;
        beforButton.onClick.AddListener(onClickBeforButton);
        nextButton.onClick.AddListener(onClickNextButton);
        show();
    }
    public override void Close()
    {
        base.Close();
        DialogueManager.Instance.offDialogue();
        playerRawImage.texture = null;
        npcRawImage.texture = null;
    }
    void onClickBeforButton()
    {
        messagesPointer = Math.Min(0, messagesPointer--);
        show();
    }
    void onClickNextButton()
    {
        messagesPointer++;
        if(messages.Count <= messagesPointer)
        {
            UIManager.Instance.Hide<DialogueBoard>();
            UIManager.Instance.Hide<ShopUI>();
            return;
        }
        show();
    }

    void show()
    {
        typingTween?.Kill(); // 이전 트윈 정리

        isTyping = true;
        Conversation.text = ""; // 초기화
        //Conversation.text = messages[messagesPointer];
        StartCoroutine(showCoroutine());
    }

    IEnumerator showCoroutine()
    {
        for(int i = 0; i < messages[messagesPointer].Length; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Conversation.text = Conversation.text + messages[messagesPointer][i];
        }        
    }
}
