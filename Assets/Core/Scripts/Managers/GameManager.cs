using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager GetInstance { get; private set; }


    private void Awake()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;
    }
}
