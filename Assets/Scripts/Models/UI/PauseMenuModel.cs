using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuModel
{
    public bool isPaused;

    public void TogglePause()
    {
        isPaused = !isPaused;
    }
}
