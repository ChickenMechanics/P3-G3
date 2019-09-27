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
        Debug.Log("Walk");
    }


    public void FixedUpdate()
    {
        //m_Owner.FixedUpdatePos(0.0f, ForceMode.Force);
    }


    public void Update()
    {
        if (Input.GetAxisRaw("Horizontal") == 0.0f &&
            Input.GetAxisRaw("Vertical") == 0.0f)
        {
            IState state = m_Owner.GetState(PlayerCtrl.EP_State.IDLE);
            m_Owner.GetFsm().ChangeState(state);
        }


        

        //UpdateIdle(dT);

        //m_Owner.IsGrounded();
        //m_Owner.UpdateGrfxRot();
        //m_Owner.UpdateMoveDir();
    }


    public void Exit()
    {

    }
}