using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObstacle :Obstacle
{
    private Vector3 velocityTmp;

    protected override void OnEnable()
    {
        base.OnEnable();

        float power = Random.Range(8f, 10f);
        // 초기 속도 설정
        rb.AddForce(Vector3.up * power, ForceMode.Impulse);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && other.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            controller.Attack(damagable);
        }
    }

    protected override void SetPlayMode()
    {
        if(!isPlaying)
        {
            velocityTmp = rb.velocity; // 현재 속도를 저장
            rb.velocity = Vector3.zero; // 게임이 일시정지되면 Rigidbody 속도 초기화
            rb.useGravity = false; // 중력 비활성화
        }
        else
        {
            rb.velocity = velocityTmp; // 게임이 일시정지되면 Rigidbody 속도 초기화
            rb.useGravity = true; // 중력 활성화
        }
    }
}
