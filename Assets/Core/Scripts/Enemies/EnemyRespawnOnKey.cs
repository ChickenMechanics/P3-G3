using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRespawnOnKey : MonoBehaviour
{
    [Header("Press 'R' for respawn")]

    public GameObject m_EnemyToSpawn;

    private GameObject m_EnemyInstance;


    private void Awake()
    {
        m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (m_EnemyInstance != null)
            {
                Debug.LogWarning("EnemyRespawnOnKeyPress::Update(): Enemy is already spawned.");
                return;
            }

            m_EnemyInstance = Instantiate(m_EnemyToSpawn, transform.position, transform.rotation, transform);
        }
    }
}
