using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;         
    public float fadeDuration = 1f; // 페이드 시간

    void Start()
    {
        StartCoroutine(FadeIn());
    }
    
    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    
    public IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsed = 0f;
        Color color = fadeImage.color;
        
        color.a = 1f;
        fadeImage.color = color;
        
        
        if (fadeDuration <= 0f)
        {
            fadeDuration = 1f; // fallback
        }
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeImage.color = color;
            
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        
    }
}
