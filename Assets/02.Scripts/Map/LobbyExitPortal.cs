using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyExitPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowConfirmPopup(
                "로비를 나가시겠습니까?",
                onConfirm: () => { 
                    SceneManager.LoadScene("MainScene"); 
                },
                onCancel: () =>
                { });
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.GetUI<ConfirmPopup>()?.Close();
        }
    }
}
