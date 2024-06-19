using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    public NPCConversation npcConversation;
    [SerializeField] float offsetLeft = 5, offsetRight = 5, offsetTop = 5, offsetBottom = 5, speedHorizontal = 1, speedVertical = 1;
    [SerializeField] public bool hasReachedRight = false, hasReachedLeft = false, hasReachedTop = false, hasReachedBottom = false;
    Vector3 startPosition = Vector3.zero;
    float defaultTop, defaultBottom, defaultLeft, defaultRight;
    public GameObject activeFruit, snake;
    Rigidbody body;
    public int bodyParts, fruitNeeded, currentStage = 1;
    public bool nextStage = false, movingFruit = false, spawned = false;

    void Awake()
    {
        startPosition = transform.position;
    }
    void Start()
    {
        defaultTop = offsetTop;
        defaultBottom = offsetBottom;
        defaultLeft = offsetLeft;
        defaultRight = offsetRight;
    }
    void LateUpdate()
    {

        if(activeFruit == null && !movingFruit)
        {
            chooseFruit();
        }
        if(movingFruit)
        {
            if (!hasReachedRight)
            {//Diagonal platforms move vertically and horizontally at the same time
                if (transform.position.x < startPosition.x + offsetRight)
                {
                    Move(offsetRight);
                }
                else if (transform.position.x >= startPosition.x + offsetRight)
                {
                    hasReachedRight = true;
                    hasReachedLeft = false;
                }
            }
            else if (!hasReachedLeft)
            {
                if (transform.position.x > startPosition.x + offsetLeft)
                {
                    Move(offsetLeft);
                }
                else if (transform.position.x <= startPosition.x + offsetLeft)
                {
                    hasReachedRight = false;
                    hasReachedLeft = true;
                }
            }
            if (!hasReachedTop)
            {
                if (transform.position.z < startPosition.z + offsetTop)
                {
                    MoveDiag(offsetTop);
                }
                else if (transform.position.z >= startPosition.z + offsetTop)
                {
                    hasReachedTop = true;
                    hasReachedBottom = false;
                }
            }
            else if (!hasReachedBottom)
            {
                if (transform.position.z > startPosition.z + offsetBottom)
                {
                    MoveDiag(offsetBottom);
                }
                else if (transform.position.z <= startPosition.z + offsetBottom)
                {
                    hasReachedTop = false;
                    hasReachedBottom = true;
                }
            }
        }
    }
    public void randomSpawnEarth()
    {//Pick a random spot on earth map to spawn fruit
        float randX = Random.Range(-16f, 16f);
        float randZ = Random.Range(-8.5f, 6.5f);
        Vector3 randomSpawnPosition = new Vector3(randX, activeFruit.transform.position.y, randZ);
        activeFruit.transform.position = randomSpawnPosition;
        spawned = true;
    }
    public void randomSpawnHell()
    {//Pick a random spot on hell map to spawn fruit
        float randX = Random.Range(-16f, 16f);
        float randZ = Random.Range(29f, 43f);
        Vector3 randomSpawnPosition = new Vector3(randX, activeFruit.transform.position.y, randZ);
        activeFruit.transform.position = randomSpawnPosition;
        spawned = true;
    }
    public void chooseFruit()
    {//Determine where to randomly spawn the fruit
        activeFruit = this.gameObject.transform.GetChild(0).gameObject;
        if (activeFruit.gameObject.tag.Contains("DragonFruit"))
        {
            randomSpawnEarth();
            activeFruit.SetActive(true);
            ConversationManager.Instance.StartConversation(npcConversation);
        }
        else if (activeFruit.gameObject.tag.Contains("Lust") || activeFruit.gameObject.tag.Contains("Greed") || activeFruit.gameObject.tag.Contains("Pride") || activeFruit.gameObject.tag.Contains("WrathFruit"))
        {
            activeFruit.SetActive(true);
        }
        else if (activeFruit.gameObject.tag.Contains("Wrath") || activeFruit.gameObject.tag.Contains("Gluttony") || activeFruit.gameObject.tag.Contains("End"))
        {
            randomSpawnHell();
            activeFruit.SetActive(true);
        }
        else if (activeFruit.gameObject.tag.Contains("Last"))
        {
            randomSpawnHell();
            //activeFruit.SetActive(true);
        }
        else
        {
            randomSpawnEarth();
            activeFruit.SetActive(true);
        }
    }
    public void EndLevel()
    {
        snake.GetComponent<Movement>().moveSpeed = 0;
        snake.GetComponent<Movement>().bodySpeed = 0;
        snake.GetComponent<Movement>().steerSpeed = 0;
    }
    public void activateFruit()
    {
        activeFruit = this.gameObject.transform.GetChild(0).gameObject;
        activeFruit.SetActive(true);
    }

    void Move(float offset)
    {
        transform.position = Vector3.MoveTowards(transform.position,
                                                    new Vector3(startPosition.x + offset,
                                                                transform.position.y,
                                                                transform.position.z),
                                                    speedHorizontal * Time.deltaTime);
    }
    void MoveDiag(float offset)
    {
        transform.position = Vector3.MoveTowards(transform.position,
                                         new Vector3(transform.position.x,
                                                     startPosition.y,
                                                     transform.position.z + offset),
                                         speedVertical * Time.deltaTime);
    }
}
