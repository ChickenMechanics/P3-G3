using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    Rigidbody m_Rb;
    Camera m_PlayerCam;
    GameObject m_PlayerCameraPoint;
    GameObject m_PlayerGunPoint;

    Vector3 m_MoveDir;
    Vector3 m_ForwardForce;
    Vector3 m_StrafeForce;

    Vector2 m_NextLookRotation;
    Vector2 m_CurrentLookRotation;

    float m_ForwardSpeed;
    float m_StrafeSpeed;
    float m_SpeedScaler;

    #region DesignerVariables
    [Header("Movement")]
    public float m_AccelerationForce = 100.0f;
    public float m_Speed = 10.0f;

    [Header("Look")]
    public float m_LookSensitivity = 4;
    [Range(0.0f, 1.0f)]
    public float m_MouseSmooth = 0.4f;
    [Range(0.0f, 100.0f)]
    public float m_LookPitchMin = 98.0f;
    [Range(-100.0f, 0.0f)]
    public float m_LookPitchMax = -98.0f;

    [Header("Gun")]
    public GameObject m_GunPreFab;
    public Vector3 m_PositionOffset = new Vector3(0.2f, 0.2f, 0.5f);
    public float m_Rotation = -5;
    #endregion

    GameObject m_GunInstance;


    void Start()
    {
        Cursor.visible = false;

        m_Rb = GetComponent<Rigidbody>();

        m_PlayerCameraPoint = GameObject.FindGameObjectWithTag("CameraPoint");
        m_PlayerCam = Camera.main;
        m_PlayerCam.transform.position = m_PlayerCameraPoint.transform.position;
        m_PlayerCam.transform.SetParent(m_PlayerCameraPoint.transform);

        m_PlayerGunPoint = GameObject.FindGameObjectWithTag("GunPoint");
        m_GunInstance = Instantiate(m_GunPreFab, m_PlayerGunPoint.transform.position, Quaternion.identity);
        m_GunInstance.transform.position = m_PlayerGunPoint.transform.position;
        m_GunInstance.transform.SetParent(m_PlayerGunPoint.transform);

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
    }


    private void Gun()
    {
        m_PlayerGunPoint.transform.eulerAngles = m_PlayerCam.transform.eulerAngles;

        m_PlayerGunPoint.transform.position = m_PlayerCam.transform.position +
            ((m_PlayerCam.transform.right * m_PositionOffset.x) +
            (-m_PlayerCam.transform.up * m_PositionOffset.y) +
            (m_PlayerCam.transform.forward * m_PositionOffset.z));
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
        m_NextLookRotation = Vector2.Lerp(m_NextLookRotation, mouseLook, m_MouseSmooth);
        m_CurrentLookRotation += m_NextLookRotation;

        if (m_CurrentLookRotation.x > 360.0f) m_CurrentLookRotation.x = 0.0f;
        else if (m_CurrentLookRotation.x < -360.0f) m_CurrentLookRotation.x = 0.0f;

        m_CurrentLookRotation.y = Mathf.Clamp(m_CurrentLookRotation.y, m_LookPitchMax, m_LookPitchMin);

        m_PlayerCam.transform.localRotation = Quaternion.AngleAxis(-m_CurrentLookRotation.y, Vector3.right);
        m_PlayerCam.transform.localRotation = Quaternion.AngleAxis(m_CurrentLookRotation.x, Vector3.up);

        transform.eulerAngles = new Vector3(0.0f, m_CurrentLookRotation.x, 0.0f);
        m_PlayerCam.transform.eulerAngles = new Vector3(-m_CurrentLookRotation.y, m_CurrentLookRotation.x, 0.0f);
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
    }


    private void FixedUpdate()
    {
        FixedMove();
    }


    private void LateUpdate()
    {
        Look();
        Gun();
    }
}
