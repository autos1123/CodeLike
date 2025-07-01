using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destinationPoint;
    public bool isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if(!isActive) return;
        if (other.CompareTag("Player"))
        {
            other.transform.position = destinationPoint.transform.position;
        }
    }
}
