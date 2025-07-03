public class EnemyAnimationEventSender : AnimationEventSender<EnemyController>
{
    public override void SendEvent()
    {
        originObject.AttackAction();
    }
}
