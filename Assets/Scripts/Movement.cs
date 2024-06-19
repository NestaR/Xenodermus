using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DialogueEditor;
using System.Linq;

public class Movement : MonoBehaviour
{
    public NPCConversation[] npcConversations;
    public float moveSpeed = 5, bodySpeed = 5, steerSpeed = 180;
    private float behindDistance = 2f;
    public int Gap = 10, score;
    int currentConv;
    public bool death = false, snake = false, haschanged = false, canMove = false, ascending = false, descending = false, invincible = false, wrath = false, fired = false;
    private bool tp = false;
    public GameObject bodyPrefab, dragonPrefab, snakePrefab, EarthObject, SpaceObject, convos, firebreath;
    public Camera mainCamera;
    public Transform pos, MainCamera, CameraPosition2, CameraPosition3, ascensionPosition, descensionPosition;
    public Vector3 bodyrotation;
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<GameObject> DeadBodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();
    private float convTimer, invincTimer, fireTimer;
    private Rigidbody dragonRigidbody;
    public GameObject EndPanel, GreedWalls;
    public Text scoreText;
    public FruitController fruitController;
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        ConversationManager.Instance.StartConversation(npcConversations[0]);
        dragonRigidbody = this.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if(snake)
        {//Movement of last snake character
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            invincible = true;
        }
        else if (!death && canMove && !ascending && !descending)
        {//Let player change directions
            dragonRigidbody.constraints = RigidbodyConstraints.None;
            dragonRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            float steerDirection = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);
            followHead();
        }
        else if (death)
        {
            resetPlayer();
        }
        if (!haschanged && !death && canMove && !ascending && !descending)
        {//Movement for first snake character
            float steerDirection = Input.GetAxis("Horizontal");
            dragonPrefab.transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);
        }

        score = BodyParts.Count * 100;//Calc score off body parts
        scoreText.text = score.ToString();
        if (GameObject.FindWithTag("Body") == null && wrath)
        {
            //SceneManager.LoadScene("EndScene");
            StartConv(7);
        }

        if (wrath && Input.GetKey("f") && !fired)
        {//Activate fire breath prefab to destroy certain objects
            StartCoroutine(WrathFire());

        }
        if (invincTimer > 5f)
        {//End invincibility
            resetSpeed();
            invincible = false;
        }
        else if (invincible)
        {//Measure invincibility time
            invincTimer += Time.deltaTime;
        }
    }
    private void GrowSnake()
    {//Eating a fruit adds a body part to the player
        //Vector3 spawnPos = transform.position - (transform.forward * behindDistance);
        Vector3 spawnPos = pos.position;
        GameObject body = Instantiate(bodyPrefab, spawnPos, transform.rotation);
        BodyParts.Add(body);
        Gap -= 2;
    }
    private void ShrinkSnake()
    {//Destroy parts of players body
        for(int i = 1; i < 4; i++)
        {
            DeadBodyParts.Add(BodyParts[BodyParts.Count() - i]);
            BodyParts.RemoveAt(BodyParts.Count() - i);           
        }    
    }
    private void ShrinkSnakeAll()
    {//Destroy remaining body parts
        DeadBodyParts.Add(BodyParts[2]);
        DeadBodyParts.Add(BodyParts[1]);
        DeadBodyParts.Add(BodyParts[0]);
        BodyParts.RemoveAt(2);
        BodyParts.RemoveAt(1);
        BodyParts.RemoveAt(0);
    }
    public void KillSnake()
    {
        death = true;
    }
    public void deactivateFire()
    {
        firebreath.SetActive(false);
    }
    public void EndConv()
    {
        Invoke("enableMovement", 1);
    }
    public void CloseConv()
    {
        //ConversationManager.Instance.EndConversation();
        convos.SetActive(false);
    }
    public void Ascend()
    {//Prepare for ascending player
        Gap -= 50;
        Vector3 myEulerAngleRotation = new Vector3(60f, 0, 0);     
        ascending = true;    
        invincible = true;
        StartCoroutine(Ascension());
    }
    public void Descend()
    {//Prepare for descending player
        descending = true;
        wrath = true;
        invincible = true;
        StartCoroutine(Descension());
    }
    public void StartConv(int convNum)
    {        
        if (ConversationManager.Instance != null)
        {//Chooses which conversation to play from various methods
            if (convNum == 1)
            {
                ConversationManager.Instance.StartConversation(npcConversations[convNum]);
            }
            else if (convNum == 2)
            {
                convos.SetActive(true);
                ConversationManager.Instance.StartConversation(npcConversations[convNum]);
            }
            else if (convNum == 3)
            {
                //convos.SetActive(true);
                //GreedWalls.SetActive(true);
                ConversationManager.Instance.StartConversation(npcConversations[convNum]);
            }
            else if (convNum == 4)
            {
                convos.SetActive(true);
                ConversationManager.Instance.StartConversation(npcConversations[convNum]);
            }
            else if (convNum == 5)
            {
                ConversationManager.Instance.StartConversation(npcConversations[convNum]);
            }
            else if (convNum == 6)
            {
                ConversationManager.Instance.StartConversation(npcConversations[convNum]);
            }
            else if (convNum == 7)
            {
                ConversationManager.Instance.StartConversation(npcConversations[convNum]);
            }
        }
    }
    public void followHead()
    {//Get all the body parts to follow the player
        if(!tp)
        {
            PositionsHistory.Insert(0, transform.position);

            int index = 0;
            foreach (var body in BodyParts)
            {
                if (body != null)
                {
                    Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];
                    Vector3 moveDirection = point - body.transform.position;
                    body.transform.position += moveDirection * bodySpeed * Time.deltaTime;
                    body.transform.LookAt(point);
                    index++;
                }
            }
        }
    }
    public void BodyColliderOn()
    {
        int index = 0;
        foreach (var body in DeadBodyParts)
        {
            if (body != null)
            {
                body.transform.GetChild(0).gameObject.GetComponent<MeteorLauncher>().final = true;
                index++;
            }
        }     
    }
    public void endTP()
    {
        tp = false;
    }

    public void enableMovement()
    {
        canMove = true;
        death = false;
    }
    public void disableMovement()
    {
        canMove = false;
    }
    public void resetPlayer()
    {//Destroy all players body parts
        foreach (var body in BodyParts)
        {
            Destroy(body);
        }
        PositionsHistory.Clear();
    }
    private IEnumerator DestroyBody()
    {//Sequence for destroying the players body and head to replace them
        GameObject[] bodyArray = new GameObject[BodyParts.Count()];
        int bodycount = 0;
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        foreach (var body in BodyParts)
        {
            bodyArray[bodycount] = body;
            bodycount++;
        }
        foreach (GameObject bp in bodyArray.Reverse())
        {
            Destroy(bp);
            yield return wait;
        }
        PositionsHistory.Clear();
        ActivateDragon();       
    }
    private IEnumerator Ascension()
    {//Create a dynamic shift to the next stage of the game
        Invoke("ActivateSpaceWalls", 4.5f);
        while(!SpaceObject.activeSelf)
        {
            invincible = true;
            transform.position += transform.forward * 6.6f * Time.deltaTime;
            dragonRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            Vector3 dragonRotation = new Vector3(0, 0, 0);
            Quaternion quaterniondragon = Quaternion.Euler(dragonRotation.x, dragonRotation.y, dragonRotation.z);
            dragonPrefab.transform.rotation = Quaternion.Slerp(dragonPrefab.transform.rotation, quaterniondragon, Time.deltaTime);

            Vector3 myEulerAngleRotation = new Vector3(60f, 0, 0);
            Quaternion quaternion = Quaternion.Euler(myEulerAngleRotation.x, myEulerAngleRotation.y, myEulerAngleRotation.z);
            EarthObject.transform.rotation = Quaternion.Slerp(EarthObject.transform.rotation, quaternion, Time.deltaTime * 0.7f);
            MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, CameraPosition2.transform.position, Time.deltaTime * 0.7f);
            mainCamera.orthographic = false;
            followHead();
            yield return null;
        }
        resetSpeed();
    }
    private IEnumerator Descension()
    {//Create a dynamic shift to the next stage of the game
        while (descending && (transform.position.y) > -41f)
        {
            invincible = true;
            GreedWalls.SetActive(false);
            transform.position += transform.up * -7f * Time.deltaTime;
            dragonRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            Vector3 dragonRotation = new Vector3(0, 0, 0);
            Quaternion quaterniondragon = Quaternion.Euler(dragonRotation.x, dragonRotation.y, dragonRotation.z);
            dragonPrefab.transform.rotation = Quaternion.Slerp(dragonPrefab.transform.rotation, quaterniondragon, Time.deltaTime * 0.8f);

            Vector3 myEulerAngleRotation = new Vector3(60f, 0, 0);
            Quaternion quaternion = Quaternion.Euler(myEulerAngleRotation.x, myEulerAngleRotation.y, myEulerAngleRotation.z);
            MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, CameraPosition3.transform.position, Time.deltaTime * 0.8f);
            mainCamera.orthographic = true;
            followHead();
            yield return null;
        }
        invincible = false;
        descending = false;
        canMove = true;
    }
    private IEnumerator WrathFire()
    {//Make the fire appear for a short amount of time and have a cooldown
        firebreath.SetActive(true);
        fired = true;
        yield return new WaitForSeconds(1);
        firebreath.SetActive(false);
        yield return new WaitForSeconds(1);
        fired = false;
    }
    public void ActivateDragon()
    {//Replace the players character prefab
        float myTargetRotationY = transform.rotation.y; //get the Y rotation from this object
        dragonPrefab.transform.position = transform.position;
        Quaternion myQuaternionDragon = Quaternion.Euler(0, transform.rotation.y, 0);
        dragonPrefab.gameObject.SetActive(true);
        Destroy(this.gameObject);
    }
    public void ActivateSnake()
    {//Replace the players character prefab
        float myTargetRotationY = transform.rotation.y; //get the Y rotation from this object
        snakePrefab.transform.position = transform.position;
        Quaternion myQuaternionDragon = Quaternion.Euler(0, transform.rotation.y, 0);
        snakePrefab.gameObject.SetActive(true);
        Destroy(this.gameObject);
    }
    public void resetSpeed()
    {//Default player speed
        moveSpeed = 5f;
        bodySpeed = 5f;
        steerSpeed = 200f;
    }
    public void ActivateSpaceWalls()
    {
        SpaceObject.SetActive(true);
        ascending = false;
        descending = false;
        invincible = false;
        Gap += 50;
        Vector3 dragonRotation = new Vector3(0, 0, 0);
        Quaternion quaterniondragon = Quaternion.Euler(dragonRotation.x, dragonRotation.y, dragonRotation.z);
        dragonPrefab.transform.rotation = Quaternion.Slerp(dragonPrefab.transform.rotation, quaterniondragon, 1);
        ConversationManager.Instance.EndConversation();
        canMove = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Fruit")
        {//Regular fruit have no effect
            Destroy(collision.gameObject);
            if(haschanged)
            {
                resetSpeed();
            }
            GrowSnake();
        }
        else if ((collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Player") && !invincible)
        {//Restarts the game on collision with walls or itself
            SceneManager.LoadScene("SampleScene");
        }
        else if (collision.gameObject.tag == "SpaceWall" && !tp)
        {
            PositionsHistory.Clear();
            tp = true;
            Vector3 newPos = new Vector3(-(transform.position.x), transform.position.y, transform.position.z);
            transform.position = newPos;
            foreach (var body in BodyParts)
            {
                if (body != null)
                {
                    //Vector3 newBodyPos = new Vector3(-(body.transform.position.x), body.transform.position.y, body.transform.position.z);
                    Vector3 newBodyPos = new Vector3(-(body.transform.position.x), body.transform.position.y, body.transform.position.z);
                    body.transform.position = newBodyPos;
                }

            }
            Invoke("endTP", 0.5f);
        }
        else if (collision.gameObject.tag == "SpaceRoof" && !tp)
        {
            PositionsHistory.Clear();
            tp = true;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, (transform.position.z - 30));
            transform.position = newPos;
            foreach (var body in BodyParts)
            {
                if (body != null)
                {
                    Vector3 newBodyPos = new Vector3(body.transform.position.x, body.transform.position.y, (body.transform.position.z - 35));
                    body.transform.position = newBodyPos;
                }

            }
            Invoke("endTP", 0.7f);
        }
        else if (collision.gameObject.tag == "SpaceFloor" && !tp)
        {
            PositionsHistory.Clear();
            tp = true;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, (transform.position.z + 30));
            transform.position = newPos;
            foreach (var body in BodyParts)
            {
                if (body != null)
                {
                    Vector3 newBodyPos = new Vector3(body.transform.position.x, body.transform.position.y, (body.transform.position.z + 35));
                    body.transform.position = newBodyPos;
                }

            }
            Invoke("endTP", 0.7f);
        }
        else if (collision.gameObject.tag == "SlothFruit")
        {//Slows player down
            Destroy(collision.gameObject);
            moveSpeed -= 1f;
            bodySpeed -= 1f;
            steerSpeed -= 15f;
            Gap += (Gap / 2);
            GrowSnake();
        }
        else if (collision.gameObject.tag == "DragonFruit" && !haschanged)
        {//Transforms player
            Destroy(collision.gameObject);
            canMove = false;
            StartCoroutine(DestroyBody());
        }
        else if (collision.gameObject.tag == "DragonFruit" && haschanged)
        {
            Destroy(collision.gameObject);
            GrowSnake();
        }
        else if (collision.gameObject.tag == "LustFruit")
        {//Speeds player up
            Destroy(collision.gameObject);
            StartConv(1);
            moveSpeed += 0.7f;
            bodySpeed += 0.7f;
            steerSpeed += 7f;
            GrowSnake();
        }
        else if (collision.gameObject.tag == "GreedSeed")
        {
            Destroy(collision.gameObject);
            resetSpeed();
            GrowSnake();
        }
        else if (collision.gameObject.tag == "GreedFruit")
        {
            Destroy(collision.gameObject);
            StartConv(2);
            moveSpeed = 2f;
            bodySpeed = 2f;
            Gap += 50;
        }
        else if (collision.gameObject.tag == "PrideFruit")
        {//Makes player invincible for short period
            Destroy(collision.gameObject);
            StartConv(3);
            invincTimer = 0;
            invincible = true;        
            //moveSpeed += 3f;
            //bodySpeed += 3f;
            //steerSpeed += 20f;
            GrowSnake();
        }
        else if (collision.gameObject.tag == "WrathFruit")
        {//Lets player breathe fire
            Destroy(collision.gameObject);
            canMove = false;
            StartConv(4);
            //moveSpeed = 5f;
            //bodySpeed = 5f;
            //steerSpeed = 200f;
            GrowSnake();
        }
        else if (collision.gameObject.tag == "WrathSeed")
        {
            Destroy(collision.gameObject);
            GrowSnake();
        }
        else if (collision.gameObject.tag == "GluttonyFruit")
        {//Destroys parts of players body
            Destroy(collision.gameObject);
            ShrinkSnake();
            BodyColliderOn();
        }
        else if (collision.gameObject.tag == "EndLevel")
        {//Destroys remains of players body
            Destroy(collision.gameObject);
            ShrinkSnakeAll();
            BodyColliderOn();
            StartConv(5);          
        }
        else if (collision.gameObject.tag == "EndFruit")
        {//Transforms player
            //SceneManager.LoadScene("EndScene");
            ActivateSnake();
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Meteor" && invincible)
        {
            Destroy(collider.gameObject);
        }
        else if ((collider.gameObject.tag == "Meteor" || collider.gameObject.tag == "Body") && !invincible)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else if (collider.gameObject.tag == "GreedWall" && !invincible)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
