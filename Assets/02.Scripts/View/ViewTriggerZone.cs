using UnityEngine;

public class ViewTriggerZone : MonoBehaviour
{
    [SerializeField] private bool toggleViewOnEnter = true;
    public bool hasEntered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasEntered) return;

        hasEntered = true;
        ViewManager.Instance?.ToggleView();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        hasEntered = false;
        ViewManager.Instance?.ToggleView();
    }
}
