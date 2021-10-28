using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//finite state machine of the agent. Currently it's state change involves no further actions
//can add player animations etcetera later.
public class FSM
{
    //fsm states
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
