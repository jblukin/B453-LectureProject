using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{

    [SerializeField] private GameObject _PowerUpPrefab;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", Random.Range(0f, 5f), Random.Range(20f, 60f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Spawn() 
    {

        Vector3 spawnPosition = new Vector3(Random.Range(-12f, 12f), Random.Range(-6f, 6f), 0.0f);

        if(preventSpawnOverlap(spawnPosition) && !PowerUpAvailable())
            Instantiate(_PowerUpPrefab, spawnPosition, Quaternion.identity, this.transform);


    }

    bool preventSpawnOverlap(Vector3 spawnPos)
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPos, 2);

        for(int i = 0; i < colliders.Length; i++) { //Checks edges of all objects within set range above to see if spawn if safe

            Collider2D currCollider = colliders[i];

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

    bool PowerUpAvailable()
    {

        if(transform.childCount > 0)
            if(transform.GetChild(0).CompareTag("PowerUp"))
                return true;

        return false;

    }


}
