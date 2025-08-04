using System.Collections;
using System.ComponentModel;
using UnityEngine;

public enum TrapType
{
    None,
    Return,
    Mine,
    Fake,
}
public class TrapControlLer:MonoBehaviour
{
    [SerializeField] private TrapType type;
    [SerializeField] private string sentence;
    [SerializeField] private ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.transform.CompareTag(TagName.Player)) return;

        UIManager.Instance.GetUI<MapTitleUI>().ShowTitle(sentence);
        switch(type)
        {
            case TrapType.None:
                return;
            case TrapType.Return:
                StageManager.Instance.ReLoadStage();
                break;
            case TrapType.Mine:
                if(particle != null)
                {
                    Instantiate(particle.gameObject, this.transform);
                    particle.Play();
                    if(other.transform.TryGetComponent(out IDamagable damagable))
                    {
                        damagable.GetDamaged(50f);
                    }
                }
                break;

            case TrapType.Fake:

                if(other.transform.TryGetComponent(out BoxCollider box))
                {
                    box.isTrigger = true;
                    StartCoroutine(Fake(box,1.5f));
                }
                return;
        }
    }

    IEnumerator Fake(BoxCollider box , float time)
    {
        yield return new WaitForSeconds(time);
        box.isTrigger = false;
    }
}
