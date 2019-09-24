﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletBehaviour : MonoBehaviour
{
    #region design vars
    [Header("Vfx")]
    public GameObject m_SurfaceCollisionVfx;
    public float m_VfxScale = 1.0f;
    [Header("Properties")]
    public float m_Speed;
    public float m_Gravity;
    public float m_MaxLifetimeInSec;
    #endregion
    
    private BulletBehaviour m_BulletBehaviour;
    private ParticleSystem m_SurfaceCollisionParticle;
    private Vector3 m_Force;
    private float m_CurrentLifeTime;


    public void InitBullet()
    {
        if (m_SurfaceCollisionVfx != null)
        {
            m_SurfaceCollisionParticle = Instantiate(m_SurfaceCollisionVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_SurfaceCollisionParticle.Stop();
            m_SurfaceCollisionParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_SurfaceCollisionParticle.transform.localScale = new Vector3(m_VfxScale, m_VfxScale, m_VfxScale);
        }

        m_CurrentLifeTime = 0.0f;
    }


    public void Fire(Transform bulletSpawnPoint, Vector3 dir, Vector3 vfxSpawnPoint)
    {
        transform.position = bulletSpawnPoint.position;

        transform.forward = dir;
        m_Force = (dir * m_Speed) + new Vector3(0.0f, m_Gravity, 0.0f);

        if(m_SurfaceCollisionParticle != null)
        {
            m_SurfaceCollisionParticle.transform.rotation = Camera.main.transform.rotation;
            m_SurfaceCollisionParticle.transform.position = vfxSpawnPoint;
        }

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
        // TODO: If time, move vfx things to it's own script
        if (m_SurfaceCollisionVfx != null)
        {
            m_SurfaceCollisionParticle.transform.position = transform.position;
            m_SurfaceCollisionParticle.Play();
        }

        // TODO: Do this better
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


    private void Update()
    {
        m_CurrentLifeTime += Time.deltaTime;
    }


    private void FixedUpdate()
    {
        transform.position += m_Force * Time.deltaTime;
        if (m_CurrentLifeTime > m_MaxLifetimeInSec)
        {
            Destroy(this);
        }
    }
}
