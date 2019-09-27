using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateIdle : IState
{
    private PlayerCtrl m_Owner;


    public P_StateIdle(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
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
        Debug.Log("Idle");

        //UpdateIdle(dT);

        //m_Owner.IsGrounded();
        //m_Owner.UpdateGrfxRot();
        //m_Owner.UpdateMoveDir();
    }


    public void Exit()
    {

    }
}
