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

    private ParticleSystem m_SelfExplaosionFireParticle;
    private ParticleSystem m_SelfExplosionSmokeParticle;


    private void Awake()
    {
        if (m_SelfExplosionFireVfx != null)
        {
            m_SelfExplaosionFireParticle = Instantiate(m_SelfExplosionFireVfx.GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
            m_SelfExplaosionFireParticle.Stop();
            m_SelfExplaosionFireParticle.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
            m_SelfExplaosionFireParticle.transform.localScale = new Vector3(m_SelfExplosionFireScale, m_SelfExplosionFireScale, m_SelfExplosionFireScale);
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
        if(m_SelfExplaosionFireParticle != null)
        {
            m_SelfExplaosionFireParticle.transform.position = transform.position;
            m_SelfExplaosionFireParticle.Play();
        }

        if (m_SelfExplosionSmokeParticle != null)
        {
            m_SelfExplosionSmokeParticle.transform.position = transform.position;
            m_SelfExplosionSmokeParticle.Play();
        }
    }
}
