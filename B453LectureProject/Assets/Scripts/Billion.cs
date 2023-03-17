using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billion : MonoBehaviour
{

    // Start is called before the first frame update

    GameObject _homeBase;

    bool _nearFlag = false;

    [SerializeField] int _maxHealth = 5;

    int _health;
    [SerializeField] private GameObject _gunBarrelRotationPoint;

    [SerializeField] private GameObject _gunBarrelEnd;

    [SerializeField] private float _billionDetectionRange = 10f;

    private Vector2 fireDir;


    void Start()
    {

        _health = _maxHealth;

        if(this.gameObject.GetComponentInChildren<SpriteRenderer>().color == Color.green)
            _homeBase = GameObject.Find("GreenBase");
        else if(this.gameObject.GetComponentInChildren<SpriteRenderer>().color == Color.blue)
            _homeBase = GameObject.Find("BlueBase");
        else if(this.gameObject.GetComponentInChildren<SpriteRenderer>().color == Color.red)
            _homeBase = GameObject.Find("RedBase");
        else if(this.gameObject.GetComponentInChildren<SpriteRenderer>().color == Color.yellow)
            _homeBase = GameObject.Find("YellowBase");

    }

    // Update is called once per frame
    void Update()
    {

        Move();

        TakeDamage();

        SelectEnemyTarget();

    }

    void TakeDamage() {

        if(Input.GetKeyDown(KeyCode.Mouse0)) {

            //Debug.Log("Ray fired!");

            RaycastHit2D hit;
            Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 15f);

            if(hit = Physics2D.Raycast(rayOrigin, Vector2.zero)) {

                //Debug.Log("Ray hit!");

                if(hit.collider.gameObject == this.gameObject) {

                    this._health--;

                    ResizeHPIndicator();

                    //Debug.Log("Flag hit!");

                }

            }

        }

    }

    void ResizeHPIndicator() {

        Transform resizeObj = transform.GetChild(0);

        resizeObj.parent = null;

        transform.localScale = new Vector3(transform.localScale.x - (transform.localScale.x/_maxHealth) + 0.15f, transform.localScale.y - (transform.localScale.y/_maxHealth) + 0.15f, transform.localScale.z);

        resizeObj.parent = this.transform;

        if(_health <= 0)
            Destroy(this.gameObject);

    }

    Transform GetClosestFlag() {

        float distanceA = 0f, distanceB = int.MaxValue;
        
        Transform closestFlag = null;

            foreach(Transform flag in _homeBase.GetComponentsInChildren<Transform>()) {

                if(!(flag.name == "GreenBase") && !(flag.name == "BlueBase")) {
                    
                    distanceA = Vector2.Distance(flag.position, this.transform.position);

                    if(distanceA < distanceB) {

                        distanceB = distanceA;
                        closestFlag = flag;

                    }

                }

            }

        return closestFlag;

    }

    void Move() {

        Transform flagToGOTO = GetClosestFlag();

        if(!_nearFlag && flagToGOTO != null && 0.25 <= (Vector2.Distance(this.transform.position, flagToGOTO.position) - 0.5))
            transform.position = Vector2.MoveTowards(transform.position, flagToGOTO.position, Time.deltaTime*3);

    }

    void SelectEnemyTarget() {

        Vector2 targetLocation = GetClosestBillion();

        fireDir = targetLocation - (Vector2)this.gameObject.transform.position;

        _gunBarrelRotationPoint.transform.up = fireDir;

    }

    Vector2 GetClosestBillion() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, _billionDetectionRange);

        Vector2 closestBillionLoc = Vector2.negativeInfinity;

        foreach (Collider2D billion in colliders) {

            if(billion.gameObject.tag == "Billion" && billion.gameObject.GetComponent<SpriteRenderer>().color != this.gameObject.GetComponent<SpriteRenderer>().color) {
                Vector2 currentBillionLoc = billion.transform.position;

                if(closestBillionLoc == Vector2.negativeInfinity)
                    closestBillionLoc = currentBillionLoc;
                else if (Vector2.Distance(currentBillionLoc, this.gameObject.transform.position) < Vector2.Distance(closestBillionLoc, this.gameObject.transform.position))
                    closestBillionLoc = currentBillionLoc;

            }

        }

        return closestBillionLoc;

    }

    void OnCollisionEnter2D(Collision2D collision) {

        if(_homeBase.name == "GreenBase" && collision.collider.CompareTag("GreenFlag"))
            _nearFlag = true;


        if(_homeBase.name == "BlueBase" && collision.collider.CompareTag("BlueFlag"))
            _nearFlag = true;


        if(_homeBase.name == "RedBase" && collision.collider.CompareTag("RedFlag"))
            _nearFlag = true;    


        if(_homeBase.name == "YellowBase" && collision.collider.CompareTag("YellowFlag"))
            _nearFlag = true;
    }

    void OnCollisionStay2D(Collision2D collision) {

        if(_nearFlag && collision.collider.CompareTag("Billion") && 0.25 <= (Vector2.Distance(this.transform.position, GetClosestFlag().position) - 0.5))
            transform.position = Vector2.MoveTowards(transform.position, transform.position, Time.deltaTime);

    }

    void OnCollisionExit2D(Collision2D collision) {

        if(_homeBase.name == "GreenBase" && collision.collider.CompareTag("GreenFlag"))
            _nearFlag = false;


        if(_homeBase.name == "BlueBase" && collision.collider.CompareTag("BlueFlag"))
            _nearFlag = false;


        if(_homeBase.name == "RedBase" && collision.collider.CompareTag("RedFlag"))
            _nearFlag = false;


        if(_homeBase.name == "YellowBase" && collision.collider.CompareTag("YellowFlag"))
            _nearFlag = false;
            
    }
}
