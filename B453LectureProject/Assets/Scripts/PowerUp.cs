using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(transform.parent == null)
            transform.parent = GameObject.Find("PowerUpManager").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        CheckForPickUp();

    }

    void CheckForPickUp()
    {

        int blueCount = 0, redCount = 0, greenCount = 0, yellowCount = 0;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3);

        foreach (Collider2D collider in colliders) {

            if(collider.gameObject.CompareTag("Billion")) {

                if(collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color == Color.blue) {

                    blueCount++;

                } else if(collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color == Color.red) {

                    redCount++;

                } else if(collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color == new Color(1.0f, 0.92f, 0.016f, 1.0f) /*Yellow*/) {

                    yellowCount++;

                } else if(collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color == Color.green) {

                    greenCount++;

                }

            }

        }

        if(blueCount >= 5) {

            GameObject.Find("BillionBasePrefab").SendMessage("CollectPowerUp");

            Destroy(gameObject);


        } else if(greenCount >= 5) {

            GameObject.Find("BillionBasePrefabGreen").SendMessage("CollectPowerUp");

            Destroy(gameObject);


        } else if(redCount >= 5) {

            GameObject.Find("BillionBasePrefabRed").SendMessage("CollectPowerUp");

            Destroy(gameObject);

        } else if(yellowCount >= 5) {

            GameObject.Find("BillionBasePrefabYellow").SendMessage("CollectPowerUp");

            Destroy(gameObject);

        }
            

    }
}
