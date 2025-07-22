using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleSceneManager : MonoBehaviour
{
    public VideoPlayer backgroundVideoPlayer;
    public RawImage backgroundVideoScreen;

    public VideoPlayer staticNoiseVideoPlayer;
    public RawImage staticNoiseVideoScreen;

    public TextMeshProUGUI pressAnyKeyText;
    
    public AudioSource bgmAudioSource;
    
    public string introSceneName = "IntroScene";
    
    public float delayBeforeNoiseVideo = 2.0f; // 노이즈 영상 재생 전 딜레이 시간

    private bool inputHandled = false;

    void Awake()
    {
        if (backgroundVideoScreen != null) backgroundVideoScreen.gameObject.SetActive(false);
        if (staticNoiseVideoScreen != null) staticNoiseVideoScreen.gameObject.SetActive(false);

        if (pressAnyKeyText != null) pressAnyKeyText.gameObject.SetActive(true);

    }

    void OnEnable()
    {
        // 배경 동영상 재생
        if (backgroundVideoPlayer != null)
        {
            if (backgroundVideoScreen != null) backgroundVideoScreen.gameObject.SetActive(true);
            backgroundVideoPlayer.Play();
        }

        // 노이즈 동영상 종료 이벤트 구독
        if (staticNoiseVideoPlayer != null)
        {
            staticNoiseVideoPlayer.loopPointReached += OnStaticNoiseVideoEnd;
        }
        
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }
    }

    void OnDisable()
    {
        if (staticNoiseVideoPlayer != null)
        {
            staticNoiseVideoPlayer.loopPointReached -= OnStaticNoiseVideoEnd;
        }
        
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }

    void Update()
    {
        // 아무 키나 입력되었고, 아직 입력이 처리되지 않았다면
        if (Input.anyKeyDown && !inputHandled)
        {
            HandleAnyKeyInput();
        }
        // (선택 사항) Press Any Key 텍스트 깜빡임 효과
        if (pressAnyKeyText != null && pressAnyKeyText.gameObject.activeSelf)
        {
            pressAnyKeyText.color = new Color(pressAnyKeyText.color.r, pressAnyKeyText.color.g, pressAnyKeyText.color.b, Mathf.PingPong(Time.time * 0.8f, 1f));
        }
    }

    void HandleAnyKeyInput()
    {
        inputHandled = true; 
        
        // "아무 키나 입력하세요" 텍스트 비활성화
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.gameObject.SetActive(false);
        }
        
        StartCoroutine(PlayStaticNoiseWithDelay());
    }
    
    IEnumerator PlayStaticNoiseWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeNoiseVideo);
        
        // 배경 동영상 정지 및 화면 비활성화
        if (backgroundVideoPlayer != null)
        {
            backgroundVideoPlayer.Stop();
        }
        if (backgroundVideoScreen != null)
        {
            backgroundVideoScreen.gameObject.SetActive(false);
        }
        
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
        
        if (staticNoiseVideoPlayer != null)
        {
            if (staticNoiseVideoScreen != null) staticNoiseVideoScreen.gameObject.SetActive(true);
            staticNoiseVideoPlayer.Play();
        }
        else
        {
            StartCoroutine(LoadIntroSceneAfterDelay(0.5f)); 
        }
    }
    void OnStaticNoiseVideoEnd(VideoPlayer vp)
    {
        // 노이즈 동영상 화면 비활성화
        if (staticNoiseVideoScreen != null)
        {
            staticNoiseVideoScreen.gameObject.SetActive(false);
        }
        
        SceneManager.LoadScene(introSceneName);
    }

    // 혹시 노이즈 영상이 없거나 바로 넘어가야 할 때 사용
    IEnumerator LoadIntroSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(introSceneName);
    }
}
