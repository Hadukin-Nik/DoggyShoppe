using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenuController : MonoBehaviour
{
    public AudioMixer mixer;

    bool settingsOpened = false;
    private PauseMenuModel model;
    public Canvas menuCanvas;
    public Canvas settingsCanvas;

    private void Start()
    {
        model = new PauseMenuModel();
        menuCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settingsOpened)
            {
                if (model.isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
            else
            {
                CloseSettings();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        model.TogglePause();
        menuCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        model.TogglePause();
        menuCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        Cursor.lockState = CursorLockMode.None;
    }

    public void OpenSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
        menuCanvas.gameObject.SetActive(false);
        settingsOpened = true;
    }
    public void CloseSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
        menuCanvas.gameObject.SetActive(true);
        settingsOpened = false;
    }
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }

    public void SetVolume(float v)
    {
        mixer.SetFloat("Volume", v);
    }
}
