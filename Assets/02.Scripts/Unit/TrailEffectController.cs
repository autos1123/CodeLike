using UnityEngine;
using System.Collections;

public class TrailEffectController:MonoBehaviour
{
    [Header("Trail Settings")]
    public float defaultTrailTime = 0.3f;

    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        if(trail != null)
            trail.enabled = false;
    }

    /// <summary>
    /// trail 효과를 일정 시간 동안 켠다.
    /// </summary>
    public void PlayTrail(float duration = -1f)
    {
        if(trail == null) return;
        StopAllCoroutines();
        trail.enabled = true;
        StartCoroutine(DisableTrailAfter(duration > 0 ? duration : defaultTrailTime));
    }

    private IEnumerator DisableTrailAfter(float time)
    {
        yield return new WaitForSeconds(time);
        trail.enabled = false;
    }
}
