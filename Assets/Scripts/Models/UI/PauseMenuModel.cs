using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MenuStates;

public class PauseMenuModel
{
    public bool isPaused = false;
    public MenuState state = MenuState.None;

    public void Pause()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        isPaused = !isPaused;
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = !isPaused;
    }
    public void Open(Canvas c)
    {
        c.gameObject.SetActive(true);
    }
    public void Close(Canvas c)
    {
        c.gameObject.SetActive(false);
    }
    public void LoadScene(string s)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(s);
    }
    
}
