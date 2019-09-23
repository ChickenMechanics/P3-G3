using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletSelfDestruction : MonoBehaviour
{
    private ParticleSystem m_Particle;
    private float m_Duration;
    private float m_TimePassed;


    private void Awake()
    {
        m_Particle = GetComponent<ParticleSystem>();
        m_Duration = m_Particle.main.duration;
        m_TimePassed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Particle.isPlaying == true)
        {
            m_TimePassed += Time.deltaTime;
            if (m_TimePassed >= m_Duration)
            {
                Destroy(this);
            }
        }
    }
}
