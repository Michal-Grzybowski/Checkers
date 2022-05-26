using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ConfigMenu : MonoBehaviour
{
    public TMP_Dropdown color_dropdown;
    public TMP_Dropdown algorithm_dropdown_1;
    public TMP_Dropdown depth_dropdown_1;
    public TMP_Dropdown algorithm_dropdown_2;
    public TMP_Dropdown depth_dropdown_2;

    private void Start()
    {
    }

    public void DropdownColorSelected()
    {
        if(color_dropdown.value == 1)
        {
            Play.IsFirstBotWhite = false;
        }
        else
        {
            Play.IsFirstBotWhite = true;
        }
    }

    public void DropdownBot1DepthSelected()
    {
        Play.FirstBotDepth = int.Parse(depth_dropdown_1.options[depth_dropdown_1.value].text);
    }

    public void DropdownBot2DepthSelected()
    {
        Play.SecondBotDepth = int.Parse(depth_dropdown_2.options[depth_dropdown_2.value].text);
    }

    public void DropdownBot1AlgorithmSelected()
    {
        Play.FirstBotAlgorithm = algorithm_dropdown_1.options[algorithm_dropdown_1.value].text;
    }

    public void DropdownBot2AlgorithmSelected()
    {
        Play.SecondBotAlgorithm = algorithm_dropdown_2.options[algorithm_dropdown_2.value].text;
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
