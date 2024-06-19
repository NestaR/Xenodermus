using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject MenuPanel;

    void Update()
    {//Open and close the menu
        if(Input.GetKeyUp("m") && MenuPanel.activeSelf)
        {
            MenuPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else if (Input.GetKeyUp("m") && !MenuPanel.activeSelf)
        {
            MenuPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void NextLevel()
    {
        SceneManager.LoadScene("Level1");
    }    
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void ExitLevel()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("SampleScene");
        //Application.Quit();
    }
}
