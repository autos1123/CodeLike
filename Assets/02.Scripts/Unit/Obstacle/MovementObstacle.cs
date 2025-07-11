using UnityEngine;

public abstract class MovementObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected Transform obstacleTransform;
    [SerializeField] protected float speed = 1f;

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        Movement();
    }

    protected abstract void Movement();
}
