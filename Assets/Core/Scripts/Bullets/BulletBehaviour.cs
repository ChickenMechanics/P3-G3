using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    #region design vars
    [Header("Bullet Properties")]
    public GameObject m_BulletModelPrefab;
    public float m_FireRate;
    public float m_Speed;
    public float m_Gravity;
    // bullet
    // vfx
    #endregion

    // Bullet things
    private GameObject m_BulletModel;
    private BulletBehaviour m_BulletBehaviour;
    private Transform m_BulletSpawnPoint;
    private Vector3 m_Dir;


    public void InitBullet()
    {
        m_BulletModel = Instantiate(m_BulletModelPrefab, Vector3.zero, Quaternion.identity);
        m_BulletModel.transform.SetParent(transform);
    }


    public void Fire(Transform spawnPoint, Vector3 dir)
    {
        m_BulletModel.transform.position = spawnPoint.position;
        m_BulletModel.transform.rotation = spawnPoint.rotation;

        m_Dir = dir;

        gameObject.SetActive(true);
    }


    private void OnEnable()
    {
        if (m_BulletModel != null)
        {
            m_BulletModel.SetActive(true);
        }
    }


    private void OnDisable()
    {
        if (m_BulletModel != null)
        {
            m_BulletModel.SetActive(false);
        }
    }


    private void Update()
    {
        transform.position += ((m_Dir * m_Speed) + new Vector3(0.0f, m_Gravity, 0.0f)) * Time.deltaTime;
    }


    private void OnDestroy()
    {
        //Destroy(m_BulletModel);
        Destroy(this);
    }
}
