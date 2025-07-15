using System.Linq;

public class StatusBoard : UIBase
{
    public override string UIName => "StatusBoard";
    Status[] _status;
    TableManager _tableManager;
    private void Awake()
    {
        _status = transform.GetComponentsInChildren<Status>();
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
