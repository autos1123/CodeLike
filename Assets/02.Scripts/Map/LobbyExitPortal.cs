using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyExitPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowConfirmPopup(
                "모험을 떠나자",
                onConfirm: () => { 
                    LoadingSceneController.LoadScene("MainScene");
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
