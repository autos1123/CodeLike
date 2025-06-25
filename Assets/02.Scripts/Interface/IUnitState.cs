using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitState
{
    void StateEnter();
    void StateExit();
    void StateUpdate();
    void StatePhysicsUpdate();
}
