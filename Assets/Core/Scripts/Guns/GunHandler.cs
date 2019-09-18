﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunHandler : MonoBehaviour
{
    GunHandler() { m_ActiveGunIdx = 0; }

    #region design vars
    public Transform m_CameraTransform;     // Acts as weapon root transform

    [Header("Gun Locker")]
    public int m_DefaultGun;
    public GameObject[] m_ProjectileGunVariantPrefabs;
    #endregion

    private GameObject[] m_ProjectileGunClones;
    private GameObject m_ActiveGun;
    private GunTemplate m_ActiveGunData;
    private int m_ActiveGunIdx;


    //----------------------------------------------------------------------------------------------------


    public GameObject GetActiveGun()
    {
        return m_ActiveGun;
    }


    public void Init()
    {
        CreateGunInstances();

        m_ActiveGun = m_ProjectileGunClones[m_ActiveGunIdx];
        m_ActiveGun.SetActive(true);
        m_ActiveGunData = m_ActiveGun.GetComponent<GunTemplate>();
    }


    public void SetActiveGun(int idx)
    {
        if (idx != m_ActiveGunIdx)
        {
            m_ActiveGunIdx = idx;

            m_ActiveGun.SetActive(false);
            m_ActiveGun = m_ProjectileGunClones[m_ActiveGunIdx];
            m_ActiveGun.SetActive(true);
            m_ActiveGunData = m_ActiveGun.GetComponent<GunTemplate>();
        }
    }


    private void CreateGunInstances()
    {
        m_ProjectileGunClones = new GameObject[m_ProjectileGunVariantPrefabs.Length];
        for(int i = 0; i < m_ProjectileGunVariantPrefabs.Length; ++i)
        {
            m_ProjectileGunClones[i] = Instantiate(m_ProjectileGunVariantPrefabs[i], Vector3.zero, Quaternion.identity);
            m_ProjectileGunClones[i].GetComponent<GunTemplate>().Init(m_CameraTransform);
            m_ProjectileGunClones[i].SetActive(false);
        }
    }


    void Start()
    {
        Init();
    }


    //void Update()
    //{

    //}
}
