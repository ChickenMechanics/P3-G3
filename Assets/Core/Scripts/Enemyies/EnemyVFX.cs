using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyVFX : MonoBehaviour
{
    #region design vars
    [Header("Vfx")]
    public GameObject m_SelfExplosionFireVfx;
    public GameObject m_SelfExplosionSmokeVfx;
    public float m_SelfExplosionFireScale= 1.0f;
    public float m_SelfExplosionSmokeScale = 1.0f;
    #endregion

    private ParticleSystem m_SelfExplosionFireParticle;
    private ParticleSystem m_SelfExplosionSmokeParticle;


    private void Awake()
    {
        if (m_SelfExplosionFireVfx != null)
        {
            m_SelfExplosionFireParticle = Instantiate(m_SelfExplosionFireVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_SelfExplosionFireParticle.Stop();
            m_SelfExplosionFireParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_SelfExplosionFireParticle.transform.localScale = new Vector3(m_SelfExplosionFireScale, m_SelfExplosionFireScale, m_SelfExplosionFireScale);
        }

        if (m_SelfExplosionSmokeVfx != null)
        {
            m_SelfExplosionSmokeParticle = Instantiate(m_SelfExplosionSmokeVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_SelfExplosionSmokeParticle.Stop();
            m_SelfExplosionSmokeParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_SelfExplosionSmokeParticle.transform.localScale = new Vector3(m_SelfExplosionSmokeScale, m_SelfExplosionSmokeScale, m_SelfExplosionSmokeScale);
        }
    }


    private void OnDestroy()
    {
        if (m_SelfExplosionFireParticle != null)
        {
            m_SelfExplosionFireParticle.transform.position = transform.position;
            m_SelfExplosionFireParticle.Play();
        }

        if (m_SelfExplosionSmokeParticle != null)
        {
            m_SelfExplosionSmokeParticle.transform.position = transform.position;
            m_SelfExplosionSmokeParticle.Play();
        }
    }
}
