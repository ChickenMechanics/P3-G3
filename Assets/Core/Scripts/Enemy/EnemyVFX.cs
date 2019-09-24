using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyVFX : MonoBehaviour
{
    #region design vars
    [Header("Vfx")]
    public GameObject m_SurfaceCollisionVfx;
    public float m_VfxScale = 1.0f;
    #endregion

    private ParticleSystem m_SurfaceCollisionParticle;


    private void Awake()
    {
        if (m_SurfaceCollisionVfx != null)
        {
            m_SurfaceCollisionParticle = Instantiate(m_SurfaceCollisionVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_SurfaceCollisionParticle.Stop();
            m_SurfaceCollisionParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_SurfaceCollisionParticle.transform.localScale = new Vector3(m_VfxScale, m_VfxScale, m_VfxScale);
        }
    }


    private void OnDestroy()
    {
        if(m_SurfaceCollisionParticle != null)
        {
            m_SurfaceCollisionParticle.transform.position = transform.position;
            m_SurfaceCollisionParticle.Play();
        }
    }
}
