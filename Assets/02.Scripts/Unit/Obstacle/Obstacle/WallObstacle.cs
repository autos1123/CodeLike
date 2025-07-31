using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObstacle : Obstacle
{
    [SerializeField] private float speed = 5f;
    private Vector3 velocityTmp;
    private bool alreadyAttacked = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        int randIdx = Random.Range(0, transform.childCount);
        for(int i = 0; i < transform.childCount; i++)
        {
            if(i == randIdx)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        Vector3 pos = transform.localPosition;
        pos.x = 0;
        transform.localPosition = pos;

        // 초기 속도 설정
        rb.velocity = Vector3.left * speed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        rb.velocity = Vector3.zero;
    }

    protected override void SetPlayMode()
    {
        if(!isPlaying)
        {
            velocityTmp = rb.velocity; // 현재 속도를 저장
            rb.velocity = Vector3.zero; // 게임이 일시정지되면 Rigidbody 속도 초기화
        }
        else
        {
            rb.velocity = velocityTmp; // 게임이 일시정지되면 Rigidbody 속도 초기화
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && other.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            if(alreadyAttacked)
                return;

            controller.Attack(damagable);
            alreadyAttacked = true;
        }
    }
}
