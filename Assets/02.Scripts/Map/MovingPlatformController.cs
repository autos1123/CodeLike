using UnityEngine;

public class MovingPlatformController:MonoBehaviour
{
    public Transform targetPos1;
    public Transform targetPos2;
    public float speed = 2f;

    private Transform currentTarget;

    void Start()
    {
        transform.position = targetPos1.position;
        currentTarget = targetPos2;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            currentTarget = (currentTarget == targetPos1) ? targetPos2 : targetPos1;
        }
    }
}
