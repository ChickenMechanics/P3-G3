using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set;}
    private int m_NextSceneIdx;
    private int m_CurrentSceneIdx;

    public enum EScene
    {
        MAIN_MENU = 0,
        OPTIONS,
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
        //GameObject.FindGameObjectWithTag("SceneTransitionFade").GetComponent<Animator>().SetTrigger("FadeOut");

        // TODO: Fix the fading between scenes
        FadeCompleteCallback();
    }


    public void FadeCompleteCallback()
    {
        SceneManager.LoadScene(m_NextSceneIdx);
        m_CurrentSceneIdx = m_NextSceneIdx;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        m_NextSceneIdx = -1;
        m_CurrentSceneIdx = -1;
    }
}
