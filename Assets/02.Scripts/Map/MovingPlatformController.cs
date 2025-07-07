using UnityEngine;

public class MovingPlatformController:MonoBehaviour
{
    public Transform targetPos1;
    public Transform targetPos2;
    public float speed = 2f;

    private Vector3 startPos;
    private Vector3 offset;

    void Start()
    {
        startPos = targetPos1.position;
        offset = targetPos2.position - targetPos1.position;
    }

    void Update()
    {
        float time = Mathf.PingPong(Time.time *  speed, 1f);
        transform.position = Vector3.Lerp(startPos, startPos + offset, time);
    }

    void OnCollisitonEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transfom.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transfom.SetParent(null);
        }
    }
}
