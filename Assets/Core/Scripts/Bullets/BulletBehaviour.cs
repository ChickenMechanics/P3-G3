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
    public float m_MaxLifetimeInSec;
    // bullet
    // vfx
    #endregion
    
    private GameObject m_BulletModel;
    private BulletBehaviour m_BulletBehaviour;
    private Transform m_BulletSpawnPoint;
    private Vector3 m_Dir;
    private float m_CurrentLifeTime;


    public void InitBullet()
    {
        m_BulletModel = Instantiate(m_BulletModelPrefab, Vector3.zero, Quaternion.identity);
        m_BulletModel.transform.SetParent(transform);

        m_CurrentLifeTime = 0.0f;
    }


    public void Fire(Transform spawnPoint, Vector3 dir)
    {
        m_BulletSpawnPoint = spawnPoint;
        m_Dir = dir;

        m_BulletModel.transform.position = m_BulletSpawnPoint.position;
        m_BulletModel.transform.rotation = m_BulletSpawnPoint.rotation;

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


    private void OnDestroy()
    {
        //Destroy(m_BulletModel);
        Destroy(this);
    }


    private void FixedUpdate()
    {
        transform.position += ((m_Dir * m_Speed) + new Vector3(0.0f, m_Gravity, 0.0f)) * Time.deltaTime;

        Debug.Log(m_CurrentLifeTime);
        m_CurrentLifeTime += Time.deltaTime;

        if (m_CurrentLifeTime > m_MaxLifetimeInSec)
        {
            
            Destroy(this);
        }

        //TODO: Destroy after max range and walls and enemies and all that
    }
}
