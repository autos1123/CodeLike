using UnityEngine;

public class PlayerAnimationEventSender:AnimationEventSender<PlayerController>
{
    public override void SendEvent()
    {
        originObject.Attack();
        SoundManager.Instance.PlaySFX(this.transform.position, SoundAddressbleName.SWORD_09);
    }
    public void SendSkillEvent()
    {
        Debug.Log("SendSkillEvent 호출됨");
        originObject.OnSkillInput?.Invoke();
    }
}
