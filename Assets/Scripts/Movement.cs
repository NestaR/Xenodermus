using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5, bodySpeed = 5, steerSpeed = 180;
    public int Gap = 10;
    public bool death = false, haschanged = false;
    public GameObject bodyPrefab, dragonPrefab;
    public Transform pos;
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        //GrowSnake();
        //GrowSnake();
    }

    // Update is called once per frame
    void Update()
    {
        if(!death)
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
    public void SwitchScenes()
    {
        SceneManager.LoadScene("Level1");
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Fruit")
        {
            Destroy(collision.gameObject);
            GrowSnake();
        }
        else if (collision.gameObject.tag == "Wall" && BodyParts.Count >= 7)
        {
            KillSnake();
            SwitchScenes();
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
