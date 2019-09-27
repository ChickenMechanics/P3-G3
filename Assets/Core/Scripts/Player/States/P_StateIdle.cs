using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateIdle : IState
{
    private PlayerCtrl m_Owner;
    //private PlayerStatus m_PlayerStatus;


    public P_StateIdle(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
        //m_PlayerStatus = m_Owner.GetPlayerStatus();
    }


    public void Enter(float dT)
    {

    }


    public void FixedUpdate(float dT)
    {
        //m_Owner.FixedUpdatePos(0.0f, ForceMode.Force);
    }


    public void Update(float dT)
    {
        Debug.Log("Idle");

        //UpdateIdle(dT);

        //m_Owner.IsGrounded();
        //m_Owner.UpdateGrfxRot();
        //m_Owner.UpdateMoveDir();
    }


    public void Exit(float dT)
    {

    }
}
