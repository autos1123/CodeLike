using TMPro;
using UnityEngine.UI;

public class DialogueBoard : UIBase
{
    TextMeshProUGUI Conversation;
    RawImage playerRawImage;
    RawImage npcRawImage;

    public override string UIName => "DialogueBoard";

    void Awake()
    {
        Conversation = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        playerRawImage = transform.GetChild(1).GetComponent<RawImage>();
        npcRawImage = transform.GetChild(2).GetComponent<RawImage>();
    }

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
