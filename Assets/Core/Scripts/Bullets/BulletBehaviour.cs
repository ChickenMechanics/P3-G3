using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    #region design vars
    [Header("Bullet Properties")]
    public GameObject m_BulletModelPrefab;
    public float m_FireRate;
    public float m_Speed;
    public float m_Gravity;
    // bullet
    // vfx
    #endregion

    // Bullet things
    private GameObject m_BulletModel;
    private BulletBehaviour m_BulletBehaviour;
    private Transform m_BulletSpawnPoint;


    public void InitBullet()
    {
        m_BulletModel = Instantiate(m_BulletModelPrefab, Vector3.zero, Quaternion.identity);
    }


    public void Ignite()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnDestroy()
    {
        Destroy(m_BulletModel);
        Destroy(this);
    }
}
