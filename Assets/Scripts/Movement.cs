using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DialogueEditor;

public class Movement : MonoBehaviour
{
    public NPCConversation myConversation;
    public float moveSpeed = 5, bodySpeed = 5, steerSpeed = 180;
    public int Gap = 10, fruitNeeded;
    public bool death = false, haschanged = false;
    public GameObject bodyPrefab, dragonPrefab;
    public Transform pos;
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();
    private float convTimer;
    public GameObject EndPanel;
    // Start is called before the first frame update
    void Start()
    {
        ConversationManager.Instance.StartConversation(myConversation);
        //GrowSnake();
        //GrowSnake();
    }

    // Update is called once per frame
    void Update()
    {
        ConversationManager.Instance.SetInt("BodyParts", BodyParts.Count);
        //Debug.Log(ConversationManager.Instance.GetInt("BodyParts"));
        if (!death)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            float steerDirection = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);

            PositionsHistory.Insert(0, transform.position);

            int index = 0;
            foreach (var body in BodyParts)
            {
                Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];
                Vector3 moveDirection = point - body.transform.position;
                body.transform.position += moveDirection * bodySpeed * Time.deltaTime;
                body.transform.LookAt(point);
                index++;
            }
        }
        else
        {
            
            
        }
        if (ConversationManager.Instance != null)
        {
            if (ConversationManager.Instance.IsConversationActive)
            {
                convTimer += Time.deltaTime;
            }
            if(convTimer > 2f)
            {
                Debug.Log("next");
                ConversationManager.Instance.PressSelectedOption();
                convTimer = 0;
            }
        }
        if (BodyParts.Count >= fruitNeeded)
        {
            EndPanel.SetActive(true);
        }
        //if (Input.GetKey("m") && MenuPanel.activeSelf)
        //{
        //    MenuPanel.SetActive(false);
        //    Time.timeScale = 1;
        //}
        //else if (Input.GetKey("m") && !MenuPanel.activeSelf)
        //{
        //    MenuPanel.SetActive(true);
        //    Time.timeScale = 0;
        //}
    }
    private void GrowSnake()
    {
        GameObject body = Instantiate(bodyPrefab, pos.position, transform.rotation);
        BodyParts.Add(body);
    }
    private void KillSnake()
    {
        death = true;
    }
    //public void NextLevel(int levelToLoad)
    //{
    //    SceneManager.LoadScene("Level" + levelToLoad.ToString());
    //}
    //public void ExitLevel()
    //{
    //    SceneManager.LoadScene("StartScene");
    //}
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Fruit")
        {
            Destroy(collision.gameObject);
            GrowSnake();
        }
        else if (collision.gameObject.tag == "Wall" && BodyParts.Count >= fruitNeeded)
        {
            KillSnake();
            //NextLevel(1);
        }
        else if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Player")
        {
            KillSnake();
        }
        else if (collision.gameObject.tag == "DragonFruit" && !haschanged)
        {
            Destroy(collision.gameObject);
            haschanged = true;
            float myTargetRotationY = transform.rotation.y; //get the Y rotation from this object
            dragonPrefab.transform.position = transform.position;
            Vector3 myEulerAngleRotation = new Vector3(0, myTargetRotationY, 0);
            dragonPrefab.transform.rotation = Quaternion.Euler(myEulerAngleRotation);
            dragonPrefab.gameObject.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
