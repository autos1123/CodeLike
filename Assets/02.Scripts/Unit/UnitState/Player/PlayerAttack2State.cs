using System.Linq;
using UnityEngine;

public class PlayerAttack2State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindowStart = 0f;
    private float comboWindowEnd = 0f;
    private float actualClipLength = 0f;
    private bool canCancel = false;

    private const float ComboWindowAbsoluteMin = 0.22f;
    private const float ComboWindowAbsoluteMax = 0.5f;

    public PlayerAttack2State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
<<<<<<< Updated upstream
        Debug.LogWarning("2222");
=======

>>>>>>> Stashed changes
        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Slash Attack").length;
        actualClipLength = baseClipLength / attackSpeed;

        float dynamicWindowLength = Mathf.Max(actualClipLength * 0.5f, ComboWindowAbsoluteMin);
        dynamicWindowLength = Mathf.Min(dynamicWindowLength, ComboWindowAbsoluteMax);

        comboWindowStart = actualClipLength * 0.18f;
        comboWindowEnd = Mathf.Min(comboWindowStart + dynamicWindowLength, actualClipLength);

        comboTimer = 0f;
        canCancel = false;
        StartAnimation(Player.AnimationData.Attack2ParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;

        if(comboTimer >= comboWindowStart && comboTimer <= comboWindowEnd)
        {
            canCancel = true;
            if(Player.InputHandler.AttackPressed && !Player.ComboBuffered)
                Player.ComboBuffered = true;

            if(Player.InputHandler.DashPressed)
            {
                stateMachine.ChangeState(stateMachine.DashState);
                return;
            }

            if(Player.ComboBuffered)
            {
                Player.ComboBuffered = false;
                stateMachine.ChangeState(stateMachine.Attack3State);
                return;
            }
        }
        else
        {
            canCancel = false;
        }

        if(comboTimer > actualClipLength)
            stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(Player.AnimationData.Attack2ParameterHash);
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        if(canCancel)
            Move(Player.InputHandler.MoveInput);
    }
}
