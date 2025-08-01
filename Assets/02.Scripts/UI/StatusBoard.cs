using System.Linq;

public class StatusBoard : UIBase
{
    public override string UIName => this.GetType().Name;
    Status[] _status;
    TableManager _tableManager;
    private void Awake()
    {
        _status = transform.GetComponentsInChildren<Status>();
    }

    public override void Open()
    {
        base.Open();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"StatusClose");
    }
    public override void Close()
    {
        base.Close();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"StatusOpen");
    }
}
