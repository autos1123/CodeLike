using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UnitStateMachine
{
    protected IUnitState currentState;
    public void ChangeState(IUnitState state)
    {
        currentState?.StateExit();
        currentState = state;
        currentState?.StateEnter();
    }
    public void Update()
    {
        currentState?.StateUpdate();
    }

    public void PhysicsUpdate()
    {
        currentState?.StatePhysicsUpdate();
    }
}
