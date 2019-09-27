using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class P_StateIdle : IState
{
    private PlayerCtrl m_Owner;

    private Vector3 m_MoveInput;


    public P_StateIdle(IController controller)
    {
        m_Owner = (PlayerCtrl)controller;
    }


    public void Enter()
    {
        Debug.Log("Idle");
    }


    public void FixedUpdate()
    {
        //m_Owner.FixedUpdatePos(0.0f, ForceMode.Force);
    }


    public void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0.0f ||
            Input.GetAxisRaw("Vertical") != 0.0f)
        {
            IState state = m_Owner.GetState(PlayerCtrl.EP_State.WALK);
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
