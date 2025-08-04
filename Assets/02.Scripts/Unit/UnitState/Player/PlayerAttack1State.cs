using System.Linq;
using UnityEngine;

public class PlayerAttack1State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindowStart = 0.2f;  // comboWindow 시작 시간 (예시값)
    private float comboWindowEnd = 0.5f;    // comboWindow 끝 시간 (예시값)

    public PlayerAttack1State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Slap Attack").length;
        float actualClipLength = baseClipLength / attackSpeed;

        // comboWindow의 구간 비율(직접 조절 가능)
        comboWindowStart = actualClipLength * 0.3f;
        comboWindowEnd = actualClipLength * 0.8f;

        comboTimer = 0f;
        StartAnimation(Player.AnimationData.Attack1ParameterHash);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;

        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f) PlayerLookAt();

        // comboWindow 구간에서만 입력을 받음
        if(comboTimer >= comboWindowStart && comboTimer <= comboWindowEnd)
        {
            if(Player.InputHandler.AttackPressed)
            {
                stateMachine.ChangeState(stateMachine.Attack2State);
                return;
            }
        }

        // 애니메이션이 끝났거나, comboWindow도 끝났으면 Idle로
        float animEnd = comboWindowEnd + 0.1f; // 70% 이후엔 애니메이션이 거의 끝났다고 가정
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
