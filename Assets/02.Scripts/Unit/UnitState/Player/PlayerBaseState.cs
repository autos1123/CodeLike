using UnityEngine;

public class PlayerBaseState:IUnitState
{
    protected PlayerStateMachine stateMachine;
    protected ConditionData data;
    protected ViewModeType viewMode;

    protected PlayerController player => stateMachine.Player;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        data = stateMachine.Player.Condition.Data;
    }

    public virtual void StateEnter()
    {
        if(ViewManager.HasInstance)
        {
            viewMode = ViewManager.Instance.CurrentViewMode;
            ViewManager.Instance.OnViewChanged += SwitchView;
        }
        else
        {
            viewMode = ViewModeType.View2D;
        }
    }

    public virtual void StateExit()
    {
        if(ViewManager.HasInstance)
        {
            ViewManager.Instance.OnViewChanged -= SwitchView;
        }
    }

    public virtual void StatePhysicsUpdate()
    {
        // 자식 클래스에서 오버라이드
    }

    public virtual void StateUpdate()
    {
        // 자식 클래스에서 오버라이드
    }

    protected void StartAnimation(int animationHash)
    {
        player._Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        player._Animator.SetBool(animationHash, false);
    }

    public void SwitchView(ViewModeType mode)
    {
        if(viewMode == mode) return;
        viewMode = mode;
    }

    /// <summary>
    /// 플레이어 이동 처리 (상태에서 호출)
    /// </summary>
    protected Vector3 Move(Vector2 input)
    {
        float speed = player.Condition.GetValue(ConditionType.MoveSpeed);

        Vector3 dir;
        if(viewMode == ViewModeType.View2D)
        {
            var r = Camera.main.transform.right; r.y = 0; r.Normalize();
            dir = r * input.x;
        }
        else
        {
            var f = Camera.main.transform.forward; f.y = 0; f.Normalize();
            var r = Camera.main.transform.right; r.y = 0; r.Normalize();
            dir = r * input.x + f * input.y;
        }

        Vector3 delta = dir * speed;
        delta.y = player._Rigidbody.velocity.y;
        player._Rigidbody.velocity = delta;
        return dir;
    }

    protected void PlayerLookAt()
    {
        Vector2 move = player.InputHandler.MoveInput;

        if(move.magnitude > 0.1f)
        {
            // 3D 시점 회전
            if(viewMode == ViewModeType.View3D)
            {
                var camForward = Camera.main.transform.forward; camForward.y = 0; camForward.Normalize();
                var camRight = Camera.main.transform.right; camRight.y = 0; camRight.Normalize();
                Vector3 moveDir = camRight * move.x + camForward * move.y;

                if(moveDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDir);
                    player.VisualTransform.rotation = Quaternion.Lerp(player.VisualTransform.rotation, targetRot, Time.deltaTime * player.VisualRotateSpeed);
                }
            }
            // 2D 시점 회전
            else if(viewMode == ViewModeType.View2D)
            {
                if(Mathf.Abs(move.x) > 0.01f)
                {
                    // x축이 정면이어야 하므로, 오른쪽 이동(+)일 때 y=90, 왼쪽(-)일 때 y=-90
                    float yAngle = move.x > 0 ? 90f : -90f;
                    player.VisualTransform.rotation = Quaternion.Euler(0, yAngle, 0);
                }
            }



        }
    }
}