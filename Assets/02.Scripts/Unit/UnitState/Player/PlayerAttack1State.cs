using System.Linq;
using UnityEngine;

public class PlayerAttack1State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindowStart = 0f;
    private float comboWindowEnd = 0f;
    private float actualClipLength = 0f;
    private bool canCancel = false;

    private const float ComboWindowAbsoluteMin = 0.22f; // 최소 콤보 구간
    private const float ComboWindowAbsoluteMax = 0.5f;  // 최대 콤보 구간

    public PlayerAttack1State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        Player.ComboBuffered = false;
        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Slap Attack").length;
        actualClipLength = baseClipLength / attackSpeed;

        // 최소~최대 보장 콤보 입력 구간
        float dynamicWindowLength = Mathf.Max(actualClipLength * 0.5f, ComboWindowAbsoluteMin);
        dynamicWindowLength = Mathf.Min(dynamicWindowLength, ComboWindowAbsoluteMax);

        comboWindowStart = actualClipLength * 0.18f;
        comboWindowEnd = Mathf.Min(comboWindowStart + dynamicWindowLength, actualClipLength);

        comboTimer = 0f;
        canCancel = false;
        StartAnimation(Player.AnimationData.Attack1ParameterHash);
<<<<<<< Updated upstream

        
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;
               
        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f) PlayerLookAt();
=======
>>>>>>> Stashed changes

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
                stateMachine.ChangeState(stateMachine.Attack2State);
                return;
            }
        }
        else
        {
            canCancel = false;
        }

        if(comboTimer > actualClipLength)
<<<<<<< Updated upstream
        {
            stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(Player.AnimationData.Attack1ParameterHash);
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        if(canCancel)
            Move(Player.InputHandler.MoveInput);
    }
}
