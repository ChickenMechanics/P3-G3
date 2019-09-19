using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunHandler : MonoBehaviour
{
    GunHandler() { m_ActiveGunIdx = 0; }

    #region design vars
    public Transform m_CameraTransform;     // Acts as weapon root transform

    [Header("Gun Locker")]
    public int m_DefaultGun;
    public GameObject[] m_ProjectileGunVariantPrefab;
    #endregion

    private GameObject[] m_ProjectileGunClones;
    private GameObject m_ActiveGun;
    private GunTemplate m_ActiveGunScr;
    private int m_ActiveGunIdx;
    private int m_NumOfGuns;


    //----------------------------------------------------------------------------------------------------


    public GameObject GetActiveGun()
    {
        return m_ActiveGun;
    }


    public int GetNumOfGuns()
    {
        return m_NumOfGuns;
    }


    public void Init()
    {
        CreateGunInstances();

        m_ActiveGunIdx = m_DefaultGun;
        m_ActiveGun = m_ProjectileGunClones[m_ActiveGunIdx];
        m_ActiveGun.SetActive(true);
        m_ActiveGunScr = m_ActiveGun.GetComponent<GunTemplate>();
    }


    public void SetActiveGun(int idx)
    {
        if (idx != m_ActiveGunIdx)
        {
            m_ActiveGunIdx = idx;

            m_ActiveGun.SetActive(false);
            m_ActiveGun = m_ProjectileGunClones[m_ActiveGunIdx];
            m_ActiveGun.SetActive(true);
            m_ActiveGunScr = m_ActiveGun.GetComponent<GunTemplate>();
        }
    }


    private void CreateGunInstances()
    {
        m_ProjectileGunClones = new GameObject[m_ProjectileGunVariantPrefab.Length];
        for (int i = 0; i < m_ProjectileGunVariantPrefab.Length; ++i)
        {
            m_ProjectileGunClones[i] = Instantiate(m_ProjectileGunVariantPrefab[i], Vector3.zero, Quaternion.identity);
            m_ProjectileGunClones[i].GetComponent<GunTemplate>().InitGun(m_CameraTransform);
            m_ProjectileGunClones[i].SetActive(false);

            //// Test
            //m_ProjectileGunClones[i].transform.SetParent(m_CameraTransform);
        }

        m_NumOfGuns = m_ProjectileGunClones.Length;
    }


    void Start()
    {
        Init();
    }
}