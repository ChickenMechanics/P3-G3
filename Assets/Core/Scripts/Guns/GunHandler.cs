using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO_ Make this a scriptable object and create it in player or wherever
public class GunHandler : MonoBehaviour
{
    GunHandler() { m_ActiveGunIdx = 0; }

    #region design vars
    [Header("Gun Locker")]
    public int m_DefaultGun;
    public GameObject[] m_ProjectileGunPrefab;
    #endregion

    private GameObject[] m_GunPrefabClones;
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
        m_ActiveGun = m_GunPrefabClones[m_ActiveGunIdx];
        m_ActiveGun.SetActive(true);
        m_ActiveGunScr = m_ActiveGun.GetComponent<GunTemplate>();
    }


    public void SetActiveGun(int idx)
    {
        if (idx != m_ActiveGunIdx)
        {
            m_ActiveGunIdx = idx;

            m_ActiveGun.SetActive(false);
            m_ActiveGun = m_GunPrefabClones[m_ActiveGunIdx];
            m_ActiveGun.SetActive(true);
            m_ActiveGunScr = m_ActiveGun.GetComponent<GunTemplate>();
        }
    }


    private void CreateGunInstances()
    {
        m_GunPrefabClones = new GameObject[m_ProjectileGunPrefab.Length];
        Transform parent = GameObject.Find("Camera Point").transform;
        for (int i = 0; i < m_ProjectileGunPrefab.Length; ++i)
        {
            m_GunPrefabClones[i] = Instantiate(m_ProjectileGunPrefab[i], Vector3.zero, Quaternion.identity);
            m_GunPrefabClones[i].SetActive(false);
            m_GunPrefabClones[i].transform.rotation = parent.transform.rotation;
            m_GunPrefabClones[i].transform.position = parent.transform.position;
            m_GunPrefabClones[i].transform.SetParent(parent);
            m_GunPrefabClones[i].GetComponent<GunTemplate>().InitGun();
        }

        m_NumOfGuns = m_GunPrefabClones.Length;
    }


    public void Fire(Vector3 dir)
    {
        m_ActiveGunScr.Fire(dir);
    }
}