using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTemplate : MonoBehaviour
{
    #region design vars
    [Header("Object Data")]
    public GameObject m_GunModelPrefab;
    public Vector3 m_PositionOffset;
    [Range(-1.0f, 1.0f)]
    public float m_RotationX;
    [Range(-1.0f, 1.0f)]
    public float m_RotationY;
    [Range(-1.0f, 1.0f)]
    public float m_RotationZ;
    private Vector3 m_Rotation = Vector3.zero;

    [Header("Properties")]
    public float m_Speed;
    // bullet
    // vfx
    #endregion

    private GameObject m_GunModel;
    [HideInInspector]
    public GunData m_GunData;


    public struct GunData
    {
        public Transform RootTransform;
        public float Speed;
    }


    //----------------------------------------------------------------------------------------------------


    public GameObject GetGunModel()
    {
        return m_GunModel;
    }


    public void Init(Transform root)
    {
        m_GunData.RootTransform = root;
        m_GunData.Speed = m_Speed;

        m_GunModel = Instantiate(m_GunModelPrefab, m_PositionOffset, Quaternion.identity);
    }


    private void Update()
    {
        m_GunModel.transform.rotation = m_GunData.RootTransform.rotation;

        Vector3 right = m_GunData.RootTransform.right * m_PositionOffset.x;
        Vector3 up = m_GunData.RootTransform.up * m_PositionOffset.y;
        Vector3 forward = m_GunData.RootTransform.forward * m_PositionOffset.z;

        m_GunModel.transform.position = m_GunData.RootTransform.position + right + up + forward;
    }
}
