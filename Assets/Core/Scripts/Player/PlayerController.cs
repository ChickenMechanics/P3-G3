using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    #region design vars
    [Header("Movement")]
    public float m_AccelerationForce = 100.0f;
    public float m_Speed = 10.0f;

    [Header("Look")]
    public float m_LookSensitivity = 4;
    [Range(0.0f, 1.0f)]
    public float m_LookSmooth = 0.4f;
    [Range(0.0f, 100.0f)]
    public float m_LookPitchMin = 98.0f;
    [Range(-100.0f, 0.0f)]
    public float m_LookPitchMax = -98.0f;
    #endregion

    Rigidbody m_Rb;
    Camera m_CameraPoint;
    GameObject m_PlayerEyePoint;
    GameObject m_GunPoint;

    Vector3 m_MoveDir;
    Vector3 m_ForwardForce;
    Vector3 m_StrafeForce;

    Vector2 m_NextLookRotation;
    Vector2 m_CurrentLookRotation;

    float m_ForwardSpeed;
    float m_StrafeSpeed;
    float m_SpeedScaler;

    // Lazy test gun
    private GunHandler m_Gunhandler;
    private int gunIdx = 0;


    void Start()
    {
        Cursor.visible = false;

        m_Rb = GetComponent<Rigidbody>();

        m_PlayerEyePoint = GameObject.FindGameObjectWithTag("CameraPoint");
        m_CameraPoint = Camera.main;
        m_CameraPoint.transform.position = m_PlayerEyePoint.transform.position;
        m_CameraPoint.transform.SetParent(m_PlayerEyePoint.transform);

        m_MoveDir = Vector3.zero;
        m_ForwardForce = Vector3.zero;
        m_StrafeForce = Vector3.zero;

        m_NextLookRotation = Vector2.zero;
        m_CurrentLookRotation = Vector2.zero;

        m_ForwardSpeed = m_AccelerationForce;
        m_StrafeSpeed = m_AccelerationForce;

        m_SpeedScaler = 50.0f;
        m_ForwardSpeed *= m_SpeedScaler;
        m_StrafeSpeed *= m_SpeedScaler;

        // Lazy test gun
        m_Gunhandler = GetComponent<GunHandler>();
    }


    private void  Move()
    {
        m_MoveDir.x = Input.GetAxisRaw("Horizontal");
        m_MoveDir.z = Input.GetAxisRaw("Vertical");

        m_ForwardSpeed = m_AccelerationForce * m_SpeedScaler;
        m_StrafeSpeed = m_AccelerationForce * m_SpeedScaler;

        if (m_MoveDir.x != 0.0f || m_MoveDir.z != 0.0f)
        {
            m_ForwardForce = transform.forward;
            m_ForwardForce *= m_ForwardSpeed * m_MoveDir.z * Time.deltaTime;

            m_StrafeForce = transform.right;
            m_StrafeForce *= m_StrafeSpeed * m_MoveDir.x * Time.deltaTime;
        }
    }


    private void Look()
    {
        Vector2 mouseLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * m_LookSensitivity;  // Y is pitch, x is yaw
        m_NextLookRotation = Vector2.Lerp(m_NextLookRotation, mouseLook, m_LookSmooth);
        m_CurrentLookRotation += m_NextLookRotation;

        if (m_CurrentLookRotation.x > 360.0f) m_CurrentLookRotation.x = 0.0f;
        else if (m_CurrentLookRotation.x < -360.0f) m_CurrentLookRotation.x = 0.0f;

        m_CurrentLookRotation.y = Mathf.Clamp(m_CurrentLookRotation.y, m_LookPitchMax, m_LookPitchMin);

        m_CameraPoint.transform.localRotation = Quaternion.AngleAxis(-m_CurrentLookRotation.y, Vector3.right);
        m_CameraPoint.transform.localRotation = Quaternion.AngleAxis(m_CurrentLookRotation.x, Vector3.up);

        transform.eulerAngles = new Vector3(0.0f, m_CurrentLookRotation.x, 0.0f);
        m_CameraPoint.transform.eulerAngles = new Vector3(-m_CurrentLookRotation.y, m_CurrentLookRotation.x, 0.0f);
    }


    private void FixedMove()
    {
        if (m_MoveDir.x != 0.0f || m_MoveDir.z != 0.0f)
        {
            if (m_Rb.velocity.magnitude < m_Speed)
            {
                m_Rb.AddForce((m_ForwardForce + m_StrafeForce), ForceMode.Force);
            }
        }
    }


    void Update()
    {
        Move();
        Look();

        // Lazy test gun
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0.0f)
        {
            ++gunIdx;
            if (gunIdx > m_Gunhandler.GetNumOfGuns() - 1) gunIdx = 0;
            m_Gunhandler.SetActiveGun(gunIdx);
        }
    }


    private void FixedUpdate()
    {
        FixedMove();
    }
}
