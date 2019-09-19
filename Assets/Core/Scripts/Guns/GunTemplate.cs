using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunTemplate : MonoBehaviour
{
    #region design vars
    [Header("Model")]
    public GameObject m_GunModelPrefab;
    public Vector3 m_PositionOffset;

    [Header("Properties")]
    public int m_FireRate;
    public int m_MagazineSize;

    [Header("Bullet Prefab")]
    public GameObject m_BulletModelPrefab;
    #endregion

    // Gun things
    private GameObject m_GunModel;
    private GunData m_GunData;
    // Ammunition things
    private List<GameObject> m_BulletClones;
    private List<BulletBehaviour> m_BulletCloneBehaviourScrs;


    public struct GunData
    {
        public Transform RootTransform;
    }


    //----------------------------------------------------------------------------------------------------


    public GameObject GetGunModel()
    {
        return m_GunModel;
    }


    public void InitGun(Transform root)
    {
        m_GunData.RootTransform = root;
        m_GunModel = Instantiate(m_GunModelPrefab, m_PositionOffset, Quaternion.identity);

        InitMagazine();


        //// Test
        //m_GunData.RootTransform = root;
        //m_GunData.Speed = m_Speed;
        //m_GunModel = Instantiate(m_GunModelPrefab, Vector3.zero, Quaternion.identity);
        //m_GunModel.transform.SetParent(gameObject.transform);
        //InitBullets();
    }


    private void InitMagazine()
    {
        m_BulletClones = new List<GameObject>();
        m_BulletCloneBehaviourScrs = new List<BulletBehaviour>();
        Vector3 spawnPos = m_GunModel.transform.GetChild(0).transform.position;

        //for (int i = 0; i < m_MagazineSize; ++i)
        //{
        //    m_BulletClones.Add(Instantiate(m_BulletModelPrefab, spawnPos, Quaternion.identity));
        //    m_BulletCloneBehaviourScrs.Add(m_BulletCloneBehaviourScrs[i].GetComponent<BulletBehaviour>());
        //    m_BulletCloneBehaviourScrs[i].InitBullet();
        //}
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
