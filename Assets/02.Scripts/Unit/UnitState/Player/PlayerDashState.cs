using UnityEngine;

public class PlayerDashState:PlayerBaseState
{
    private float dashDuration = 0.25f; // 대쉬 지속 시간
    private float elapsedTime = 0f;

    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        if(!Player.Condition.UseStamina(15f))
        {
            Debug.LogWarning("스테미너가 부족함!");
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        StartAnimation(Player.AnimationData.DashParameterHash);
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
