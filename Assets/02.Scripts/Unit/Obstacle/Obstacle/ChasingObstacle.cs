using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingObstacle :Obstacle
{
    [SerializeField] private float rotationSpeed = 90f; // 회전 속도
    private float speedTmp;

    protected override void OnEnable()
    {
        base.OnEnable();

        transform.localRotation = Quaternion.identity;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    protected override void SetPlayMode()
    {
        
        if(isPlaying)
        {
            rotationSpeed = speedTmp;
        }
        else
        {
            speedTmp = rotationSpeed; // 현재 회전 속도를 저장
            rotationSpeed = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && other.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            controller.Attack(damagable); // 공격 처리
        }
    }
}
