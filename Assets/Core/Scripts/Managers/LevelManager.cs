using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// TODO: Make this singelton
public class LevelManager : MonoBehaviour
{
    private Animator m_FadeAnim;
    private int m_NextSceneIdx;
    private int m_CurrentSceneIdx;

    public enum EScene
    {
        MAIN_MENU = 0,
        ARENA
    }


    public void ChangeScene(EScene scene)
    {
        if (m_CurrentSceneIdx == (int)scene)
        {
            Debug.LogError("LevelManager::ChangeScene(): Next scene index and current scene index are the same. No scene change made!");
            return;
        }

        m_NextSceneIdx = (int)scene;
        m_FadeAnim.SetTrigger("FadeOut");
    }


    public void FadeSceneCallback()
    {
        SceneManager.LoadScene(m_NextSceneIdx);
        m_CurrentSceneIdx = m_NextSceneIdx;
    }


    private void Awake()
    {
        m_FadeAnim = GetComponent<Animator>();
        m_NextSceneIdx = -1;
        m_CurrentSceneIdx = -1;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeScene(EScene.ARENA);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeScene(EScene.MAIN_MENU);
        }
    }
}
