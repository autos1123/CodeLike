using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject optionPanel;   
    public GameObject loadPanel; 
    
    public Button defaultSelectedButton; //메뉴 진입 시 기본으로 선택될 버튼
    public AudioSource bgmAudioSource; 

    void Awake()
    {
        // 모든 왼쪽 페이지 패널들을 초기에는 비활성화
        if (optionPanel != null) optionPanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(false);
        
    }
    void OnEnable()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }

        // 초기 선택 버튼 설정
        if (defaultSelectedButton != null)
        {
            defaultSelectedButton.Select();
        }
    }
    void OnDisable()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }
    // 모든 왼쪽 페이지 패널을 비활성화
    void HideAllLeftPanels()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(false);
    }
    
    public void OnStartGameButton()
    {
        SceneManager.LoadScene("TutorialScene"); 
    }

    public void OnLoadGameButton()
    {
        HideAllLeftPanels(); // 다른 패널 숨기기
        if (loadPanel != null) loadPanel.SetActive(true);
    }

    public void OnOptionButton()
    {
        HideAllLeftPanels(); 
        if (optionPanel != null) optionPanel.SetActive(true);
    }
    
    public void OnExitGameButton()
    {
        #if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
        #else 
        Application.Quit();
        #endif
    }
}
