using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToLobbyButton:MonoBehaviour
{
    public void OnClickExitToLobbyButton()
    {
        SceneManager.LoadScene("TestIntroScene");

    }
}
