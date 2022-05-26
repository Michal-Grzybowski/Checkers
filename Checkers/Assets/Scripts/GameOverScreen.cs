using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text resultText;

    public void EndGame(int result)
    {
        gameObject.SetActive(true);
        string text = "";
        if(result == 1)
        {
            text = "White wins";
        }
        else if (result == 0)
        {
            text = "It's a draw";
        }
        else if(result == -1)
        {
            text = "Black wins";
        }
        resultText.text = text;
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
