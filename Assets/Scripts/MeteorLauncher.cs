using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorLauncher : MonoBehaviour
{
    public GameObject meteorPrefab, meteorObject, meteorPointer;
    public float thrust = 10f;
    private float launchTimer, lifeTimer;
    public bool stationary = false, final = false, body = false;
    Rigidbody meteorRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!stationary)
        {
            if (launchTimer > Random.Range(5, 10))
            {
                Vector3 launchPos = new Vector3(Random.Range(-20f, 20f), transform.position.y, transform.position.z);
                meteorObject = Instantiate(meteorPrefab, launchPos, transform.rotation, this.transform);
                meteorPointer.transform.position = new Vector3(launchPos.x, meteorPointer.transform.position.y, meteorPointer.transform.position.z);
                meteorRigidbody = meteorObject.GetComponent<Rigidbody>();
                launchTimer = 0;
            }
            else
            {
                launchTimer += Time.deltaTime;
            }
            if (meteorRigidbody != null && lifeTimer < 15f)
            {
                meteorRigidbody.AddForce(transform.forward * thrust);
                lifeTimer += Time.deltaTime;
            }
            else
            {
                Destroy(meteorObject);
                lifeTimer = 0;
            }
        }

    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Fire" && final && body)
        {//Enables the player to burn its own body parts
            Destroy(this.transform.parent.gameObject);         
        }
        else if (collider.gameObject.tag == "Fire" && !body)
        {//Lets player use fire to destroy certain objects
            Destroy(this.gameObject);
        }
    }
}
