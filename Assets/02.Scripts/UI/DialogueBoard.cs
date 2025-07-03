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

    }
    public override void Close()
    {
        base.Close();
    }

}
