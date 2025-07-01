using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Collider physicalCollider;

    private readonly HashSet<GameObject> trackedPlayers = new();

    private void OnTriggerStay(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        trackedPlayers.Add(other.gameObject);

        var input = other.GetComponent<PlayerInputHandler>();
        var rb = other.attachedRigidbody;
        if (input == null || rb == null) return;

        if (rb.velocity.y < -0.1f)
        {
            Physics.IgnoreCollision(other, physicalCollider, false);
        }

        else if (input.IsPressingDown())
        {
            Physics.IgnoreCollision(other, physicalCollider, true);
        }

        else
        {
            Physics.IgnoreCollision(other, physicalCollider, true);
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        if (trackedPlayers.Contains(other.gameObject))
        {
            Physics.IgnoreCollision(other, physicalCollider, false );
            trackedPlayers.Remove(other.gameObject);  
        }
    }
  }
