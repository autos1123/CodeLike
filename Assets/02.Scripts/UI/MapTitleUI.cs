using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapTitleUI : UIBase
{
    public override string UIName => this.GetType().Name;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image backgroundImage;
    
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float displayDuration = 2f;

    private Coroutine currentRoutine;

    /// <summary>
    /// 맵 이름 표시
    /// </summary>
    /// <param name="title">표시할 제목</param>
    public void ShowTitle(string title)
    {
        titleText.text = title;

        // 중복 재생 방지
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        gameObject.SetActive(true);
        currentRoutine = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {        
        SetAlpha(0f);

        // Fade In
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1f);
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        gameObject.SetActive(false);
    }
    
    private void SetAlpha(float alpha)
    {
        // 텍스트 알파
        var textColor = titleText.color;
        textColor.a = alpha;
        titleText.color = textColor;
        
        // 배경 알파
        float maxBGAlpha = 255f / 255f;
        var bgColor = backgroundImage.color;
        bgColor.a = Mathf.Clamp01(alpha) * maxBGAlpha;
        backgroundImage.color = bgColor;
    }
    
    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }
}
