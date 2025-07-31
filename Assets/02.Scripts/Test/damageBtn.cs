using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class damageBtn : MonoBehaviour
{
    public Button testButton;
    public Transform playerTransform;
    public float damage = 10f;

    private void Start()
    {
        testButton.onClick.AddListener(() =>
        {
            if (playerTransform.TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.GetDamaged(damage);
            }
        });
    }
}
