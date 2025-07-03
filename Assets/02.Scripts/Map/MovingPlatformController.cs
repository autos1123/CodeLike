using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public bool loop = true;

    private Vector3 target;
    private bool goingToB = true;

    void Start()
    {
        if (pointA != null && pointB != null)
            target = pointB.position; 
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            goingToB = !goingToB;
            target = goingToB ?  pointB.position : pointA.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            other.transform.SetParent(null);
    }
}
