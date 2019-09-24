using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEnemyBasic : MonoBehaviour
{
    #region design vars
    public float m_ScoreValue;
    private float m_Health = 100.0f;
    #endregion

    private ScoreManager m_ScoreManager;


    private void Awake()
    {
        m_ScoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BulletBasic")
        {
            // TODO: Fix health - bullet dmg
            m_Health -= 100.0f;
            if(m_Health >= 0.0f)
            {
                m_ScoreManager.AddComboPoints(m_ScoreValue);
            }
        }
    }
}
