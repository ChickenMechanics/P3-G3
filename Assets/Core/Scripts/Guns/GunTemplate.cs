using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTemplate : MonoBehaviour
{
    #region design vars
    [Header("Gun Properties")]
    public GameObject m_GunModelPrefab;
    public Vector3 m_PositionOffset;
    [Range(-1.0f, 1.0f)]
    public float m_RotationX;
    [Range(-1.0f, 1.0f)]
    public float m_RotationY;
    [Range(-1.0f, 1.0f)]
    public float m_RotationZ;
    private Vector3 m_Rotation = Vector3.zero;

    [Header("Bullet Properties")]
    public GameObject m_BulletModelPrefab;
    public float m_FireRate;
    public float m_Speed;
    public float m_Gravity;
    // bullet
    // vfx
    #endregion

    // Gun thingss
    private GameObject m_GunModel;
    private GunData m_GunData;

    // Bullet things
    private List<GameObject> m_BulletInstances;
    private BulletBehaviour m_BulletBehaviour;
    private Transform m_BulletSpawnPoint;

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


    public void InitGun(Transform root)
    {
        m_GunData.RootTransform = root;
        m_GunData.Speed = m_Speed;

        m_GunModel = Instantiate(m_GunModelPrefab, m_PositionOffset, Quaternion.identity);

        InitBullets();
    }


    private void InitBullets()
    {
        //m_BulletInstances.Add(Instantiate(m_BulletModelPrefab, m_BulletSpawnPoint.position, m_BulletSpawnPoint.rotation));
        m_BulletSpawnPoint = m_GunModel.transform.GetChild(0);
    }


    private void OnEnable()
    {
        if(m_GunModel != null)
        {
            m_GunModel.SetActive(true);
        }
    }


    private void OnDisable()
    {
        if (m_GunModel != null)
        {
            m_GunModel.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_GunModel.SetActive(false);
        }
    }


    private void Update()
    {
        m_GunModel.transform.rotation = m_GunData.RootTransform.rotation;

        Vector3 offsetPos = (m_GunData.RootTransform.right * m_PositionOffset.x) +
            (m_GunData.RootTransform.up * m_PositionOffset.y) +
            (m_GunData.RootTransform.forward * m_PositionOffset.z);

        m_GunModel.transform.position = m_GunData.RootTransform.position + offsetPos;
    }
}
