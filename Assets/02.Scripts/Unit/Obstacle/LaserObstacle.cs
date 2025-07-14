using UnityEngine;

public class LaserObstacle : MonoBehaviour
{
    [Header("Laser Obstacle Visual Settings")]
    [SerializeField] private Transform laserTransform;
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;

    [Header("Laser Obstacle Settings")]
    [SerializeField] private float damageInterval = 1f; // Time between damage applications
    [SerializeField] private float damage = 100f;

    private IDamagable player;

    // Start is called before the first frame update
    void Start()
    {
        float distance = Vector3.Distance(startTransform.position, endTransform.position);

        laserTransform.position = startTransform.position;
        laserTransform.LookAt(endTransform.position);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, distance / 2);

        InvokeRepeating(nameof(OnAttack), damageInterval, damageInterval);
    }

    private void OnAttack()
    {
        if(player != null)
        {
            player.GetDamaged(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamagable>(out IDamagable damagable) && ((1 << other.gameObject.layer) & LayerMask.GetMask("Player")) != 0)
        {
            player = damagable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<IDamagable>(out IDamagable damagable) && ((1 << other.gameObject.layer) & LayerMask.GetMask("Player")) != 0)
        {
            if(player == damagable)
            {
                player = null;
            }
        }
    }
}
