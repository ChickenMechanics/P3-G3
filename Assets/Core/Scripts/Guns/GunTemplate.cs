using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTemplate : MonoBehaviour
{
    #region design vars
    [Header("Object Data")]
    public GameObject m_GunModelPrefab;
    public Vector3 m_Position;
    public Vector3 m_Rotation;

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
        public Quaternion Rotation;
        public Vector3 Position;
        public float Speed;
    }


    //----------------------------------------------------------------------------------------------------


    public GameObject GetGunModel()
    {
        return m_GunModel;
    }


    private void Start()
    {
        m_GunData.Rotation = Quaternion.LookRotation(m_Rotation, Vector3.up);
        m_GunData.Position = m_Position;
        m_GunData.Speed = m_Speed;

        m_GunModel = Instantiate(m_GunModelPrefab, m_Position, Quaternion.identity);
    }


    private void Update()
    {
        m_GunModel.transform.position = m_GunData.Position + m_Position;
        m_GunModel.transform.rotation = m_GunData.Rotation;
    }
}
