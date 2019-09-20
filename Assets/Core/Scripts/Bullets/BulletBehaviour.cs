using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletBehaviour : MonoBehaviour
{
    #region design vars
    [Header("Bullet Properties")]
    //public GameObject m_BulletModelPrefab;
    //public float m_FireRate;
    public float m_Speed;
    public float m_Gravity;
    public float m_MaxLifetimeInSec;
    // bullet
    // vfx
    #endregion
    
    //private GameObject m_BulletModel;
    private BulletBehaviour m_BulletBehaviour;
    private Transform m_BulletSpawnPoint;
    private Vector3 m_Dir;
    private float m_CurrentLifeTime;


    public void InitBullet()
    {
        //m_BulletModel = Instantiate(m_BulletModelPrefab, Vector3.zero, Quaternion.identity);
        //m_BulletModel.transform.SetParent(transform);

        m_CurrentLifeTime = 0.0f;
    }


    public void Fire(Transform spawnPoint, Vector3 dir)
    {
        m_BulletSpawnPoint = spawnPoint;
        m_Dir = dir;

        transform.position = m_BulletSpawnPoint.position;
        transform.rotation = m_BulletSpawnPoint.rotation;

        gameObject.SetActive(true);
    }


    private void OnEnable()
    {
        gameObject.SetActive(true);
    }


    private void OnDisable()
    {
        gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        Destroy(this);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(this);
        }
        else if(other.CompareTag("DestroyBullet"))
        {
            Destroy(this);
            Destroy(other.gameObject);
        }
    }


    private void FixedUpdate()
    {
        transform.position += ((m_Dir * m_Speed) + new Vector3(0.0f, m_Gravity, 0.0f)) * Time.deltaTime;

        m_CurrentLifeTime += Time.deltaTime;
        if (m_CurrentLifeTime > m_MaxLifetimeInSec)
        {
            Destroy(this);
        }
    }
}
