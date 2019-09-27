using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FSM
{
    private IState m_CurrentState;


    public FSM(IState initState)
    {
        m_CurrentState = initState;
        m_CurrentState.Enter(Time.deltaTime);
    }


    public void ChangeState(IState state, float dT)
    {
        m_CurrentState.Exit(dT);
        m_CurrentState = state;
        m_CurrentState.Enter(dT);
    }


    public void Update(float dT)
    {
        if(m_CurrentState != null)
        {
            m_CurrentState.Update(dT);
        }
    }


    public void FixedUpdate(float dT)
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.FixedUpdate(dT);
        }
    }
}

