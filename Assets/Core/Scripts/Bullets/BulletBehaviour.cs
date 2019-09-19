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
    private List<GameObject> m_BulletInstances;
    private BulletBehaviour m_BulletBehaviour;
    private Transform m_BulletSpawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
