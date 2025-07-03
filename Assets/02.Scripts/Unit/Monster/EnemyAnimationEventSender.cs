using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventSender : AnimationEventSender<EnemyController>
{
    public override void SendEvent()
    {
        originObject.AttackAction();
    }
}
