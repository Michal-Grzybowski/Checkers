using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public void RunPlayerVsPlayer()
    {        
        SceneManager.LoadScene("Game");
    }
    public void RunPlayerVsAI()
    {
        Play.GameType = 1;
        SceneManager.LoadScene("Game");
    }
    public void RunAIvsAI()
    {
        Play.GameType = 2;
        SceneManager.LoadScene("Game");
    }
    public void AIConfiguration() {
        SceneManager.LoadScene("ConfigurationMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
