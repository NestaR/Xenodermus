using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject MenuPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp("m") && MenuPanel.activeSelf)
        {
            Debug.Log("test2");
            MenuPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else if (Input.GetKeyUp("m") && !MenuPanel.activeSelf)
        {
            Debug.Log("test1");
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
}
