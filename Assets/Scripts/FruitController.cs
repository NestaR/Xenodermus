using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    public GameObject activeFruit, snake;
    Rigidbody body;
    void Start()
    {
        
    }
    void Update()
    {
        if(this.gameObject.transform.GetChild(0).gameObject.tag == "End")
        {
            snake.GetComponent<Movement>().steerSpeed = 0;
        }
        else if (this.gameObject.transform.GetChild(0).gameObject.tag == "EndLevel")
        {
            Invoke("EndLevel", 0.5f);
        }
        else
        {
            activeFruit = this.gameObject.transform.GetChild(0).gameObject;
            activeFruit.SetActive(true);
        }
    }
    public void EndLevel()
    {
        snake.GetComponent<Movement>().moveSpeed = 0;
        snake.GetComponent<Movement>().bodySpeed = 0;
        snake.GetComponent<Movement>().steerSpeed = 0;
    }
}
