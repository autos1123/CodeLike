using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public event Action OnTutorialFinished;
    
    [SerializeField] private List<TutorialStep> tutorialSteps;
    [SerializeField] private string prototypeSceneName = "PrototypeScene";
    
    private int currentStepIndex = -1;
    private TutorialStep currentStep;

    protected override bool Persistent => false;

    protected override void Awake()
    {
        base.Awake();
    }

    private IEnumerator Start()
    {
        Debug.Log("튜토리얼 시작 ");
        yield return new WaitUntil(() => UIManager.Instance != null && UIManager.Instance.IsUILoaded()); 
        yield return new WaitUntil(() => TutorialStageManager.Instance != null);
        
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
        
    }
    private void EndTutorial()
    {
        if (currentStep != null)
        {
            currentStep.OnStepCompleted -= NextStep;
            currentStep.Deactivate();
        }

        UIManager.Instance.HideContextualHint();
        Debug.Log("모든 튜토리얼 스텝 완료. 다음 씬으로 전환.");
        SceneManager.LoadScene(prototypeSceneName);
    }
}
