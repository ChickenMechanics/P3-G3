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
    private Transform m_BulletSpawnPoint;
    // Ammunition things
    private List<GameObject> m_BulletPrefabClones;
    private List<BulletBehaviour> m_BulletBehaviourScripts;
    private int m_NextFreeBullet;


    public struct GunData
    {
        public Transform RootTransform;
    }


    //----------------------------------------------------------------------------------------------------


    private void Awake()
    {
        m_BulletPrefabClones = new List<GameObject>();
        m_BulletBehaviourScripts = new List<BulletBehaviour>();
    }


    public GameObject GetGunModel()
    {
        return m_GunModel;
    }


    public void InitGun(Transform root)
    {
        m_GunData.RootTransform = root;
        m_GunModel = Instantiate(m_GunModelPrefab, m_PositionOffset, Quaternion.identity);
        m_BulletSpawnPoint = m_GunModel.transform.GetChild(0);

        InitMagazine();
    }
    

    private void InitMagazine()
    {
        if(m_BulletBehaviourScripts.Count > 0)  m_BulletBehaviourScripts.Clear();
        if (m_BulletPrefabClones.Count > 0)     m_BulletPrefabClones.Clear();

        Vector3 spawnPos = m_GunModel.transform.GetChild(0).transform.position;
        Quaternion spawnRot = m_GunModel.transform.GetChild(0).rotation;

        for (int i = 0; i < m_MagazineSize; ++i)
        {
            GameObject bulletClone = Instantiate(m_BulletModelPrefab, spawnPos, spawnRot);
            BulletBehaviour bulletScr = bulletClone.GetComponent<BulletBehaviour>();

            bulletScr.InitBullet();
            bulletClone.SetActive(false);

            m_BulletPrefabClones.Add(bulletClone);
            m_BulletBehaviourScripts.Add(bulletScr);
        }

        m_NextFreeBullet = m_MagazineSize - 1;
    }


    public void Fire(Vector3 dir)
    {
        if(m_BulletBehaviourScripts.Count == 0)
        {
            Debug.LogError("GunTemplate::Fire(): No bollit clones in magazine!");
            return;
        }

        BulletBehaviour bulletScr = m_BulletBehaviourScripts[m_NextFreeBullet];
        GameObject bulletClone = m_BulletPrefabClones[m_NextFreeBullet];

        bulletScr.Fire(m_BulletSpawnPoint, dir);
        m_BulletBehaviourScripts.Remove(bulletScr);
        m_BulletPrefabClones.Remove(bulletClone);

        if (m_NextFreeBullet == 0) return;
        --m_NextFreeBullet;
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
