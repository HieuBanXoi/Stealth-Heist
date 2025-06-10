using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainHallManager : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void LoadSetting()
    {
        SceneManager.LoadScene("Setting");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
