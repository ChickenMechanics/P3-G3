using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour, IController
{
    public enum EP_State
    {
        IDLE = 0,
        WALK,
        RUN,
        DEAD,
        SIZE
    }

    private IState[] m_States;
    private FSM m_FSM;
    private IState m_IdleState;
    private IState m_WalkState;


    private void Awake()
    {
        m_States = new IState[(int)EP_State.SIZE];
        m_States[(int)EP_State.IDLE] = new P_StateIdle((IController)this);
        m_States[(int)EP_State.WALK] = new P_StateWalk((IController)this);

        m_FSM = new FSM(m_States[(int)EP_State.IDLE]);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            m_FSM = new FSM(m_States[(int)EP_State.IDLE]);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_FSM = new FSM(m_States[(int)EP_State.WALK]);
        }

        m_FSM.Update(Time.deltaTime);
    }

}
