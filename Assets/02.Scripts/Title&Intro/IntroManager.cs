using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public GameObject menuCanvas;
    public CanvasGroup uiFadeGroup;
    public static event Action OnMenuFadeInComplete; 

    void OnEnable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    public void PlayVideo()
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/Intro_book4.mp4";

        if (videoPlayer != null)
        {
            if (!videoPlayer.isPrepared) 
            {
                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += (source) => {
                    videoPlayer.Play();
                };
            }
            else
            {
                videoPlayer.Play();
            }
        }
        else
        {
            Debug.LogError("비디오플레이어가 없습니다."); 
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }
        
        if (uiFadeGroup != null)
        {
            uiFadeGroup.alpha = 0f;
            uiFadeGroup.interactable = false;
            uiFadeGroup.blocksRaycasts = false;

            StartCoroutine(FadeInCanvasGroup(uiFadeGroup, 2f, 2f)); 
        }
        
        // 동영상 오브젝트를 비활성화하거나 파괴하여 화면에서 제거
        // vp.gameObject.SetActive(false); 
    }
    
    IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null; 
        }
        
        canvasGroup.alpha = targetAlpha; 
        canvasGroup.interactable = true; 
        canvasGroup.blocksRaycasts = true; 
        
        OnMenuFadeInComplete?.Invoke();
    }
}
