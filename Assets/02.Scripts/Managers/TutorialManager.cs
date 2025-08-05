using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private List<TutorialStep> tutorialSteps;
    [SerializeField] private string nextSceneName = "LobbyScene";
    
    private int currentStepIndex = -1;
    private TutorialStep currentStep;
    
    private bool _isViewChangeInputAllowed = false; // 기본적으로 V키 입력은 막음
    public bool IsViewChangeInputAllowed()
    {
        return _isViewChangeInputAllowed;
    }
    public void SetViewChangeInputAllowed(bool allowed)
    {
        _isViewChangeInputAllowed = allowed;
        Debug.Log($"[TutorialManager] V키 입력 허용 상태: {_isViewChangeInputAllowed}");
    }
    protected override bool Persistent => false;

    protected override void Awake()
    {
        base.Awake();
        SetViewChangeInputAllowed(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UIManager.Instance != null && UIManager.Instance.IsUILoaded()); 
        yield return new WaitUntil(() => OtherStageManager.Instance != null);
        
        StartTutorial();
    }

    void Update()
    {
        if (currentStep is TutorialInputStep inputStep)
        {
            inputStep.CheckInput();
        }
    }
    private void StartTutorial()
    {
        currentStepIndex = -1;
        NextStep();
    }

    public void NextStep()
    {
        if (currentStep != null)
        {
            currentStep.OnStepCompleted -= NextStep;
            currentStep.Deactivate();
        }

        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }

        currentStep = tutorialSteps[currentStepIndex];
        currentStep.OnStepCompleted += NextStep;
        currentStep.Activate();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"TutorialHint");
    }
    public void NotifyStepActivated(string hintMessage, string questDescription)
    {
        if (!string.IsNullOrEmpty(hintMessage))
        {
            UIManager.Instance.ShowContextualHint(hintMessage);
        }
    }
    private void EndTutorial()
    {
        if (currentStep != null)
        {
            currentStep.OnStepCompleted -= NextStep;
            currentStep.Deactivate();
        }

        UIManager.Instance.HideContextualHint();

        PlayerPrefs.SetInt("TutorialCompleted", 1); // 튜토리얼 완료로 설정
        SceneManager.LoadScene(nextSceneName);
    }
}
