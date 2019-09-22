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
    private Vector3 m_Force;
    private float m_CurrentLifeTime;


    public void InitBullet()
    {
        m_CurrentLifeTime = 0.0f;
    }


    public void Fire(Transform spawnPoint, Vector3 dir)
    {
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        m_Force = (dir * m_Speed) + new Vector3(0.0f, m_Gravity, 0.0f);

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
        else if (other.CompareTag("DestroyBullet"))
        {
            Destroy(this);
        }
    }


    private void FixedUpdate()
    {
        transform.position += m_Force * Time.fixedDeltaTime;

        m_CurrentLifeTime += Time.fixedDeltaTime;
        if (m_CurrentLifeTime > m_MaxLifetimeInSec)
        {
            Destroy(this);
        }
    }
}
