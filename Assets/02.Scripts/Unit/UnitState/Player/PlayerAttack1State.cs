using System.Linq;
using UnityEngine;

public class PlayerAttack1State:PlayerBaseState
{
    private float comboTimer = 0f;
    private float comboWindowStart = 0f;
    private float comboWindowEnd = 0f;
    private float actualClipLength = 0f;

    private const float MinComboWindow = 0.25f; // WebGL이면 더 넉넉하게!

    public PlayerAttack1State(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();

        float attackSpeed = Player.Condition.GetTotalCurrentValue(ConditionType.AttackSpeed);
        Player.Animator.SetFloat("AttackSpeed", attackSpeed);

        float baseClipLength = Player.Animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(x => x.name == "Slap Attack").length;
        actualClipLength = baseClipLength / attackSpeed;

        comboWindowStart = actualClipLength * 0.3f;
        comboWindowEnd = Mathf.Max(comboWindowStart + MinComboWindow, actualClipLength * 0.8f);
        comboWindowEnd = Mathf.Min(comboWindowEnd, actualClipLength);

        comboTimer = 0f;
        StartAnimation(Player.AnimationData.Attack1ParameterHash);

        Debug.Log($"[Attack1/StateEnter] attackSpeed:{attackSpeed}, baseClip:{baseClipLength}, actualClip:{actualClipLength}, window:{comboWindowStart:F2}~{comboWindowEnd:F2}");
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        comboTimer += Time.deltaTime;

        Debug.Log($"[Attack1/Update] comboTimer:{comboTimer:F2}, comboWindow:{comboWindowStart:F2}~{comboWindowEnd:F2}, Buffered:{Player.ComboBuffered}");

        Vector2 move = Player.InputHandler.MoveInput;
        if(move.magnitude > 0.1f) PlayerLookAt();

        // comboWindow 구간에서 "ComboBuffered"를 소비(초기화)하면서 콤보 진행
        if(comboTimer >= comboWindowStart && comboTimer <= comboWindowEnd)
        {
            if(Player.ComboBuffered)
            {
                Debug.Log("[Attack1] ComboBuffered=true! Attack2로 전환!");
                Player.ComboBuffered = false;
                stateMachine.ChangeState(stateMachine.Attack2State);
                return;
            }
        }


        if(comboTimer > actualClipLength)
        {
            Debug.Log("[Attack1] IdleState로 전환");
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
