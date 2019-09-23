using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
    #region design vars
    [Header("Model")]
    public Vector3 m_PositionOffset;

    [Header("Properties")]
    //public bool m_AutoFire;
    public int m_RoundsPerMinute;
    public int m_MagazineSize;

    [Header("Bullet Prefab")]
    public GameObject m_BulletModelPrefab;
    #endregion

    // Gun things
    private Transform m_BulletSpawnPoint;
    // Ammunition things
    private List<GameObject> m_BulletPrefabClones;
    private List<BulletBehaviour> m_BulletBehaviourScripts;
    private GameObject m_BulletParent;
    private RaycastHit m_RaycastHit;
    private float m_RayMaxDist;
    private float m_Rpm;
    private float m_TimePastSinceLastFire;
    private int m_NextFreeBullet;


    //----------------------------------------------------------------------------------------------------


    public void InitGun()
    {
        m_BulletPrefabClones = new List<GameObject>();
        m_BulletBehaviourScripts = new List<BulletBehaviour>();

        m_BulletParent = new GameObject("Bullets");
        m_BulletParent.transform.position = new Vector3(5.0f, -10.0f, 0.0f);

        m_RaycastHit = new RaycastHit();

        m_RayMaxDist = 1000.0f;
        m_Rpm = 60.0f / m_RoundsPerMinute;
        m_TimePastSinceLastFire = m_Rpm;

        //m_GunModel = Instantiate(m_GunModelPrefab, m_PositionOffset, Quaternion.identity);
        //m_GunModel.transform.position = transform.position;
        //m_GunModel.transform.SetParent(transform);
        //m_BulletSpawnPoint = m_GunModel.transform.GetChild(0);

        InitMagazine();
    }


    private void InitMagazine()
    {
        if (m_BulletBehaviourScripts.Count > 0) m_BulletBehaviourScripts.Clear();
        if (m_BulletPrefabClones.Count > 0) m_BulletPrefabClones.Clear();

        //Vector3 spawnPos = m_GunModel.transform.GetChild(0).transform.position;
        //Quaternion spawnRot = m_GunModel.transform.GetChild(0).rotation;

        for (int i = 0; i < m_MagazineSize; ++i)
        {
            //GameObject bulletClone = Instantiate(m_BulletModelPrefab, spawnPos, spawnRot);
            //BulletBehaviour bulletScr = bulletClone.GetComponent<BulletBehaviour>();

            //bulletScr.InitBullet();
            //bulletClone.SetActive(false);
            //bulletClone.transform.SetParent(m_BulletParent.transform);

            //m_BulletPrefabClones.Add(bulletClone);
            //m_BulletBehaviourScripts.Add(bulletScr);
        }

        m_NextFreeBullet = m_MagazineSize - 1;
    }


    private void UpdateMagazine()
    {
        // Time is resetted in fire

        if (m_TimePastSinceLastFire < m_Rpm)
            m_TimePastSinceLastFire += Time.deltaTime;
    }


    private void UpdateTransform()
    {
        Vector3 offsetPos = (transform.right * m_PositionOffset.x) +
                            (transform.up * m_PositionOffset.y) +
                            (transform.forward * m_PositionOffset.z);

        //m_GunModel.transform.position = transform.position + offsetPos;
    }


    public void Fire(Vector3 dir)
    {
        if (m_BulletBehaviourScripts.Count == 0)
        {
            Debug.LogError("GunTemplate::Fire(): No bollit in clip!");
            return;
        }

        if (m_TimePastSinceLastFire >= m_Rpm)
        {
            Ray ray = new Ray(transform.position, dir);
            Vector3 raycastedDir = dir;
            if (Physics.Raycast(ray, out m_RaycastHit, m_RayMaxDist))
            {
                raycastedDir = (m_RaycastHit.point - m_BulletSpawnPoint.position).normalized;
            }

            BulletBehaviour bulletScr = m_BulletBehaviourScripts[m_NextFreeBullet];
            GameObject bulletClone = m_BulletPrefabClones[m_NextFreeBullet];

            bulletScr.Fire(m_BulletSpawnPoint, raycastedDir, m_RaycastHit.point);
            m_BulletBehaviourScripts.Remove(bulletScr);
            m_BulletPrefabClones.Remove(bulletClone);

            if (m_NextFreeBullet == 0) return;

            --m_NextFreeBullet;
            m_TimePastSinceLastFire = 0.0f;
        }
    }


    private void OnEnable()
    {
        //if (m_GunModel != null)
        //{
        //    m_GunModel.SetActive(true);

        //    Vector3 offsetPos = (transform.right * m_PositionOffset.x) +
        //                        (transform.up * m_PositionOffset.y) +
        //                        (transform.forward * m_PositionOffset.z);

        //    m_GunModel.transform.rotation = transform.rotation;
        //    m_GunModel.transform.position = transform.position + offsetPos;
        //}
    }


    private void OnDisable()
    {
        //if (m_GunModel != null)
        //{
        //    m_GunModel.SetActive(false);
        //    m_GunModel.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
        //}
    }


    private void Update()
    {
        //if (m_GunModel != null)
        //{
        //    UpdateTransform();
        //    UpdateMagazine();
        //}
    }
}