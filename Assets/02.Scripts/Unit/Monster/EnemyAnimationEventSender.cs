
public class EnemyAnimationEventSender : AnimationEventSender<EnemyController>
{
    public override void SendEvent()
    {
        originObject.AttackAction();
        SoundManager.Instance.PlaySFX(this.transform.position, SoundAddressbleName.SWORD_09);
    }
}
