using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateWalk : IState
{
    private PlayerCtrl m_Owner;
    //private PlayerStatus m_PlayerStatus;


    public P_StateWalk(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
        //m_PlayerStatus = m_Owner.GetPlayerStatus();
    }


    public void Enter()
    {

    }


    public void FixedUpdate()
    {
        //m_Owner.FixedUpdatePos(0.0f, ForceMode.Force);
    }


    public void Update()
    {
        Debug.Log("Walk");

        //UpdateIdle(dT);

        //m_Owner.IsGrounded();
        //m_Owner.UpdateGrfxRot();
        //m_Owner.UpdateMoveDir();
    }


    public void Exit()
    {

    }
}