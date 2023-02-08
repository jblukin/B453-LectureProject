using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillionBase : MonoBehaviour
{

    [SerializeField] private GameObject _billionPrefab;

    private Collider2D[] _colliders;

    private Color _color;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnBillion", 0.0f, 1.0f);

        if(this.gameObject.name == "GreenBase") {

            _color = Color.green;

        } else if(this.gameObject.name == "BlueBase") {

            _color = Color.blue;

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnBillion() 
    {

        Vector3 spawnPos = new Vector3();

        bool canSpawnHere = false;

        int attempts = 0;

        while(!canSpawnHere) {

            spawnPos = new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), 0.0f);

            canSpawnHere = preventOverlap(spawnPos);

            attempts++;

            if(canSpawnHere)
                break;

            if(attempts > 50)
                break;
        }

        if(canSpawnHere) {

            GameObject billion = Instantiate(_billionPrefab, spawnPos, Quaternion.identity);

            billion.GetComponent<SpriteRenderer>().color = _color;

        }

    }

    bool preventOverlap(Vector3 spawnPos) //Prevents Initial Spawn Overlap
    {

        _colliders = Physics2D.OverlapCircleAll(transform.position, 5);

        for(int i = 0; i < _colliders.Length; i++) { //Checks edges of all objects within set range above to see if spawn if safe

            Collider2D currCollider = _colliders[i];

            Vector3 centerPoint = currCollider.bounds.center;
            float width = currCollider.bounds.extents.x;
            float height = currCollider.bounds.extents.y;

            float leftExtent = centerPoint.x - width;
            float rightExtent = centerPoint.x + width;

            float lowerExtent = centerPoint.y - height;
            float upperExtent = centerPoint.y + height;

            if(spawnPos.x >= leftExtent && spawnPos.x <= rightExtent)
                if(spawnPos.y >= lowerExtent && spawnPos.y <= upperExtent)
                    return false;

        }

        return true;

    }


}
