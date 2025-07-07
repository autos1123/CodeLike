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
        data = stateMachine.Player.Data;
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
        if(ViewManager.HasInstance != null && ViewManager.Instance.IsTransitioning)
            return Vector3.zero;

        float speed = 0f;
        if(!data.TryGetCondition(ConditionType.MoveSpeed, out speed))
        {
            Debug.LogWarning("[PlayerBaseState] MoveSpeed가 설정되어 있지 않습니다.");
        }

        Vector3 dir;
        if(viewMode == ViewModeType.View2D)
        {
            dir = new Vector3(input.x, 0f, 0f);
        }
        else
        {
            var f = Camera.main.transform.forward; f.y = 0; f.Normalize();
            var r = Camera.main.transform.right; r.y = 0; r.Normalize();
            dir = r * input.x + f * input.y;
        }

        Vector3 delta = dir * speed * Time.fixedDeltaTime;
        player._Rigidbody.MovePosition(player._Rigidbody.position + delta);
        return dir;
    }
}