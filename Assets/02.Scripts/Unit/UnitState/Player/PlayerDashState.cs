using UnityEngine;

public class PlayerDashState:PlayerBaseState
{
    private float dashDuration = 0.25f; // 대쉬 지속 시간
    private float elapsedTime = 0f;

    // 쿨타임 관련 추가 변수
    private float dashCooldown = 2f; // 쿨타임 시간
    private float lastDashTime = -Mathf.Infinity;

    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    // 대쉬 가능 여부 체크 메소드
    private bool CanDash()
    {
        return Time.time >= lastDashTime + dashCooldown;
    }

    public override void StateEnter()
    {
        // 먼저 쿨타임 체크
        if(!CanDash())
        {
            Debug.LogWarning("대쉬가 아직 쿨타임 중입니다!");
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        // 스테미너 체크
        if(!Player.Condition.UseStamina(15f))
        {
            Debug.LogWarning("스테미너가 부족함!");
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        // 위의 조건이 모두 만족했을 때만 Base의 진입 로직 호출
        base.StateEnter();

        lastDashTime = Time.time; // 대쉬 사용 시간 기록

        StartAnimation(Player.AnimationData.DashParameterHash);
        SoundManager.Instance.PlaySFX(Player.transform.position, SoundAddressbleName.DashSound);
        elapsedTime = 0f;
        Player._Rigidbody.useGravity = false;
        Player._Rigidbody.velocity = Vector3.zero;

        Vector2 input = Player.InputHandler.MoveInput;
        Vector3 dir;
        if(viewMode == ViewModeType.View2D)
        {
            dir = new Vector3(input.x, 0, 0).normalized;
        }
        else
        {
            var f = Camera.main.transform.forward; f.y = 0; f.Normalize();
            var r = Camera.main.transform.right; r.y = 0; r.Normalize();
            dir = (r * input.x + f * input.y).normalized;
        }

        if(dir == Vector3.zero)
        {
            dir = Player.VisualTransform.forward;
        }

        float dashPower = 15f;
        Player._Rigidbody.AddForce(dir * dashPower, ForceMode.VelocityChange);

        if(Player.DashVFXPrefab != null)
        {
            Vector3 spawnPosition = Player.transform.position
                + Vector3.up
                - dir * 0.8f;
            Quaternion rotation = Quaternion.LookRotation(dir);

            GameObject vfx = GameObject.Instantiate(
                Player.DashVFXPrefab,
                spawnPosition,
                rotation
            );

            GameObject.Destroy(vfx, 2f); // 자동 제거
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= dashDuration)
        {
            if(Player.InputHandler.MoveInput != Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.MoveState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
    }

    public override void StateExit()
    {
        base.StateExit();
        Player._Rigidbody.velocity = Vector3.zero;
        StopAnimation(Player.AnimationData.MoveParameterHash);
        Player._Rigidbody.useGravity = true;
    }

    public override void StatePhysicsUpdate()
    {
        base.StatePhysicsUpdate();
    }
}
