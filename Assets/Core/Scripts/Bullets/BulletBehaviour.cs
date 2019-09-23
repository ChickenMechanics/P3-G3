using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletBehaviour : MonoBehaviour
{
    #region design vars
    [Header("Bullet Properties")]
    public GameObject m_SurfaceCollisionVfx;
    public float m_Speed;
    public float m_Gravity;
    public float m_MaxLifetimeInSec;
    // bullet
    // vfx
    #endregion
    
    //private GameObject m_BulletModel;
    private BulletBehaviour m_BulletBehaviour;
    private ParticleSystem m_SurfaceCollisionParticle;
    private Vector3 m_Force;
    private Vector3 m_VfxSpawnPoint;
    private float m_CurrentLifeTime;


    public void InitBullet()
    {
        if (m_SurfaceCollisionVfx != null)
        {
            m_SurfaceCollisionParticle = Instantiate(m_SurfaceCollisionVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_SurfaceCollisionParticle.Stop();
            m_SurfaceCollisionParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_SurfaceCollisionParticle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }


        m_CurrentLifeTime = 0.0f;
    }


    public void Fire(Transform spawnPoint, Vector3 dir, Vector3 vfxSpawnPoint)
    {
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        m_Force = (dir * m_Speed) + new Vector3(0.0f, m_Gravity, 0.0f);
        m_VfxSpawnPoint = vfxSpawnPoint;

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
            // TODO: Move particles to better place
            if (m_SurfaceCollisionVfx != null)
            {
                m_SurfaceCollisionParticle.transform.position = m_VfxSpawnPoint;
                m_SurfaceCollisionParticle.Play();
            }


            Destroy(other.gameObject);
            Destroy(this);
        }
        else if (other.CompareTag("DestroyBullet"))
        {
            // TODO: Move particles to better place
            if (m_SurfaceCollisionVfx != null)
            {
                m_SurfaceCollisionParticle.transform.position = m_VfxSpawnPoint;
                m_SurfaceCollisionParticle.Play();
            }

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
