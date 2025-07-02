using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destinationPoint;
    public float teleportCooldown = 1.0f;

    private Dictionary<GameObject, float> lastTeleportTimes = new();

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        GameObject player = other.gameObject;
        float currentTime = Time.time;

        if (lastTeleportTimes.ContainsKey(player))
        {
            float lastTime = lastTeleportTimes[player];
            if(currentTime - lastTime < teleportCooldown)
                return;
        }

        other.transform.position = destinationPoint.position;
        lastTeleportTimes[player] = currentTime;

        Portal targetPortal = destinationPoint.GetComponentInParent<Portal>();
        if (targetPortal != null )
        {
            targetPortal.lastTeleportTimes[player] = currentTime;
        }
    }
 }
