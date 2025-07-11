using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventSender:AnimationEventSender<PlayerController>
{
    public override void SendEvent()
    {
        originObject.Attack();
        SoundManager.Instance.PlaySFX(this.transform.position, SoundAddressbleName.SWORD_09);
    }
}
