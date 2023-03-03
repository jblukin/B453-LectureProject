using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillionBaseGreen : MonoBehaviour
{

    [SerializeField] private GameObject _billionPrefab;

    [SerializeField] private GameObject[] _flagPrefabs;

    private int _flagsPlaced = 0;

    private Collider2D[] _colliders;

    private Color _color;

    private GameObject _currentFlag = null;

    private bool _canMoveFlag = false;
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
        spawnInitialFlags();

        if(_canMoveFlag)
            MoveFlag();

    }

    void SpawnBillion() 
    {

        Vector3 spawnPos = new Vector3();

        bool canSpawnHere = false;

        int attempts = 0;

        while(!canSpawnHere) {

            spawnPos = new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), 0.0f);

            canSpawnHere = preventSpawnOverlap(spawnPos);

            attempts++;

            if(canSpawnHere)
                break;

            if(attempts > 50)
                break;
        }

        if(canSpawnHere) {

            GameObject billion = Instantiate(_billionPrefab, spawnPos, Quaternion.identity);

            billion.GetComponentInChildren<SpriteRenderer>().color = _color;
        }

    }
    bool preventSpawnOverlap(Vector3 spawnPos) //Prevents Initial Spawn Overlap
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

    void spawnInitialFlags() 
    {

        if(_flagsPlaced < 2 && Input.GetKeyDown(KeyCode.Mouse0)) {

            if((Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1)) && _color == Color.green) {

                Instantiate(_flagPrefabs[1], Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 11f), Quaternion.identity, this.gameObject.transform);

                _flagsPlaced++;

            }
            else if((Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2)) && _color == Color.blue) {

                Instantiate(_flagPrefabs[0], Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 11f), Quaternion.identity, this.gameObject.transform);

                _flagsPlaced++;

            }
        }

        if(_flagsPlaced == 2 && _canMoveFlag == false)
            StartCoroutine(DelayRayCast());

    }

    IEnumerator DelayRayCast()
    {

        yield return new WaitForSeconds(0.25f);
        _canMoveFlag = true;

    }

    void MoveFlag() 
    {

        if(Input.GetKeyDown(KeyCode.Mouse0) && _flagsPlaced == 2 && _currentFlag == null) {

            //Debug.Log("Ray fired!");

            RaycastHit2D hit;
            Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 15f);

            if(hit = Physics2D.Raycast(rayOrigin, Vector2.zero)) {

                //Debug.Log("Ray hit!");

                if(hit.collider.gameObject.tag == "GreenFlag") {

                    _currentFlag = hit.collider.gameObject;

                    _currentFlag.GetComponent<Flag>().lineToMouse.enabled = true;

                    //Debug.Log("Flag hit!");

                }

            }

        }

        else if(Input.GetKeyDown(KeyCode.Mouse0) && _currentFlag != null && _currentFlag.GetComponent<Flag>().lineToMouse.enabled == true) {

            _currentFlag.GetComponent<Flag>().lineToMouse.enabled = false;
            _currentFlag.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 15f);
            _currentFlag = null;

        }

        
    }


}
