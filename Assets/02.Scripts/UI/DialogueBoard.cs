using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoard : UIBase
{
    [SerializeField] TextMeshProUGUI Conversation;
    [SerializeField] RawImage playerRawImage;
    [SerializeField] RawImage npcRawImage;

    public override string UIName => this.GetType().Name;

    public override void Open()
    {
        base.Open();
        playerRawImage.texture = DialogueManager.Instance.playerTexture;
        npcRawImage.texture = DialogueManager.Instance.npcTexture;
    }
    public override void Close()
    {
        base.Close();
        playerRawImage.texture = null;
        npcRawImage.texture = null;
    }

}
