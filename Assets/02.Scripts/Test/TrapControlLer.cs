using UnityEngine;

public enum TrapType
{
    None,
    Return,
    Mine
}
public class TrapControlLer : MonoBehaviour
{
    public TrapType type;
    [SerializeField] private ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.transform.CompareTag(TagName.Player)) return;

        switch(type)
        {
            case TrapType.None:
                UIManager.Instance.GetUI<MapTitleUI>().ShowTitle("함정인가?");
                break;
            case TrapType.Return:
                UIManager.Instance.GetUI<MapTitleUI>().ShowTitle("처음부터 다시~");
                StageManager.Instance.ReLoadStage();
                break;
            case TrapType.Mine:
                if(particle != null)
                {
                    UIManager.Instance.GetUI<MapTitleUI>().ShowTitle("!!뿜!!");
                    particle.Play();
                    if(other.transform.TryGetComponent(out IDamagable enemy))
                    {
                        enemy.GetDamaged(50f);
                    }
                }
                else
                {
                    UIManager.Instance.GetUI<MapTitleUI>().ShowTitle("??뿜??");
                }
                break;
        }
    }
}
