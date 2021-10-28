using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    public enum FSMState : int
    {
        Idle, Move, Action
    }
    FSMState currentState;
    public FSM(FSMState s)
    {
        this.currentState = s;
    }
    public void SetState(FSMState state)
    {
        currentState = state;
    }
    public FSMState GetState()
    {
        return currentState;
    }
}
