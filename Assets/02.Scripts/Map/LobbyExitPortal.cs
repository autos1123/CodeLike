using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LobbyExitPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowConfirmPopup(
                "마을을 떠나시겠습니까?",
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
