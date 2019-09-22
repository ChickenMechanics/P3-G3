using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuGUI : MonoBehaviour
{
    public void NewGame()
    {
        LevelManager.Instance.ChangeScene(LevelManager.EScene.ARENA);
    }


    public void Options()
    {
        LevelManager.Instance.ChangeScene(LevelManager.EScene.OPTIONS);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
