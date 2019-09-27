using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour, IController
{
    #region player states
    private IState[] m_States;
    private IState m_IdleState;
    private IState m_WalkState;
    private FSM m_FSM;
    #endregion

    private PlayerLook m_PlayerLook;

    public enum EP_State
    {
        IDLE = 0,
        WALK,
        RUN,
        DEAD,
        SIZE
    }


    private void Awake()
    {
        m_States = new IState[(int)EP_State.SIZE];
        m_States[(int)EP_State.IDLE] = new P_StateIdle((IController)this);
        m_States[(int)EP_State.WALK] = new P_StateWalk((IController)this);

        m_FSM = new FSM(m_States[(int)EP_State.IDLE]);

        m_PlayerLook = GetComponent<PlayerLook>();
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
