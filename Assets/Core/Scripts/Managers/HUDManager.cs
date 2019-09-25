using UnityEngine;
using System.Collections;


public class HUDManager : MonoBehaviour
{
    public static HUDManager GetInstance { get; private set; }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;
    }
}
