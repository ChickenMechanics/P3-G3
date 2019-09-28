using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    #region design vars
    public float m_MoveAcceleration = 100.0f;
    public float m_MaxMoveSpeed = 10.0f;
    #endregion

    private Rigidbody m_Rb;

    private Vector3 m_MoveDir;
    private Vector3 m_ForwardForce;
    private Vector3 m_StrafeForce;

    private float m_ForwardAccel;
    private float m_StrafeAccel;
    private float m_AccelScaler;
    private float m_CurrentAccel;
    private float m_EyePointOffsetZ;


    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();

        m_MoveDir = Vector3.zero;
        m_ForwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;

        m_AccelScaler = 50.0f;
        m_ForwardAccel = m_MoveAcceleration * m_AccelScaler;
        m_StrafeAccel = m_MoveAcceleration * m_AccelScaler;
    }
}
