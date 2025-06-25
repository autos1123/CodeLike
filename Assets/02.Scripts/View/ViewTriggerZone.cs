using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTriggerZone : MonoBehaviour
{
    [SerializeField] private bool toggleViewOnEnter = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (toggleViewOnEnter)
        {
            ViewManager.Instance?.ToggleView();
        }
        
    }
}
