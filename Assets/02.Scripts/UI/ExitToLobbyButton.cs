using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToLobbyButton:MonoBehaviour
{
    public void OnClickExitToLobbyButton()
    {

        // 팝업으로 사용자에게 물어보고 이동하는 경우
        ShowConfirmPopup(
            "모든 정보를 잃고 로비씬으로 돌아갑니다",
            () => { SceneManager.LoadScene("LobbyScene"); }, 
            () => { }  
        );
    }

    void ShowConfirmPopup(string message, System.Action onConfirm, System.Action onCancel)
    {
        bool userConfirmed = true; 
        if(userConfirmed) onConfirm();
        else onCancel();
    }
}
