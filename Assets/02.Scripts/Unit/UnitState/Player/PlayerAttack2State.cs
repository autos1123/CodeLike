using System.Linq;
using UnityEngine;

public class PlayerAttack2State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindowStart = 0.2f;
    private float comboWindowEnd = 0.5f;

    public PlayerAttack2State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Slash Attack").length;
        float actualClipLength = baseClipLength / attackSpeed;

        comboWindowStart = actualClipLength * 0.3f;
        comboWindowEnd = actualClipLength * 0.8f;

        comboTimer = 0f;
        StartAnimation(Player.AnimationData.Attack2ParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;

        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f) PlayerLookAt();

        if(comboTimer >= comboWindowStart && comboTimer <= comboWindowEnd)
        {
            if(Player.InputHandler.AttackPressed)
            {
                stateMachine.ChangeState(stateMachine.Attack3State);
                return;
            }
        }

        float animEnd = comboWindowEnd + 0.1f;
        if(comboTimer > animEnd)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void StateExit() { base.StateExit(); }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
        Move(Player.InputHandler.MoveInput);
    }
}
