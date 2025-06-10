using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingMenuUI;
    [SerializeField] private GameObject backToPauseMenu;
    [SerializeField] private GameObject pauseButton;
    // Update is called once per frame
    public void Start()
    {
        // pauseMenuUI.SetActive(false);
        // settingMenuUI.SetActive(false);
        // backToPauseMenu.SetActive(false);
    }
    void Update()
    {

    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
    }
    public void Setting()
    {
        pauseMenuUI.SetActive(false);
        settingMenuUI.SetActive(true);
        backToPauseMenu.SetActive(true);
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        pauseButton.SetActive(true);
    }
    public void BackToPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        settingMenuUI.SetActive(false);
        backToPauseMenu.SetActive(false);
    }
}
