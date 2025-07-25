public class PlayerAnimationEventSender:AnimationEventSender<PlayerController>
{
    public override void SendEvent()
    {
        originObject.Attack();
        SoundManager.Instance.PlaySFX(this.transform.position, SoundAddressbleName.SWORD_09);
    }
}
