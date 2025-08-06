using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public abstract string UIName { get; }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        this.transform.SetAsLastSibling();
    }
    public virtual void Close() => gameObject.SetActive(false);
}
