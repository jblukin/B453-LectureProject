using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillionImproved : MonoBehaviour
{

    private GameObject _homeBase;

    [SerializeField] private float _billionDetectionRange = 10f;

    [SerializeField] private GameObject _gunBarrelRotationPoint;

    [SerializeField] private GameObject _gunBarrelEnd;

    [SerializeField] private GameObject _bulletPrefab;

    private Vector2 _fireDir;

    [SerializeField] int _maxHealth = 5;

    private int _health;

    [SerializeField] private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {

        _health = _maxHealth;

        InvokeRepeating("Shoot", 1.0f, 3.0f);

    }

    // Update is called once per frame
    void Update()
    {

        Move();

        AimBarrel();

        TakeDamage();


    }

    void SetHomeBase(GameObject homeBase) { 
        
        _homeBase = homeBase;
    
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

    void TakeBulletDamage(int amount) {

        _health -= amount;

        ResizeHPIndicator();

    }

    void ResizeHPIndicator() {

        Transform resizeObj = transform.Find("HPIndicator");

        resizeObj.parent = null;

        transform.localScale = new Vector3(transform.localScale.x - (transform.localScale.x/_maxHealth) + 0.15f, transform.localScale.y - (transform.localScale.y/_maxHealth) + 0.15f, transform.localScale.z);

        resizeObj.parent = this.transform;

        if(_health <= 0)
            Destroy(this.gameObject);

    }

    Transform GetClosestFlag() {

        float distanceA = 0f, distanceB = int.MaxValue;
        
        Transform closestFlag = this.transform;

            foreach(Transform flag in _homeBase.GetComponentsInChildren<Transform>()) {

                if(flag.CompareTag("Flag")) {
                    
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

        if(!NearFlagCheck()) {

            Vector2 targetLocation = GetClosestFlag().position;

            Vector2 moveDir = targetLocation - (Vector2)this.gameObject.transform.position;

            moveDir.Normalize();

            _rb.AddForce(moveDir / 2f);

        }

    }

    bool NearFlagCheck() {

        if(_homeBase.transform.childCount > 0) { 

            Transform[] flags = _homeBase.GetComponentsInChildren<Transform>();
        
            foreach (Transform flag in flags) {
                if(flag.CompareTag("Flag") && Vector2.Distance(transform.position, flag.position) <= 0.75f) { 
                    
                    _rb.AddForce(-_rb.velocity);
                    
                    return false;

                } else if(flag.CompareTag("Flag") && Vector2.Distance(transform.position, flag.position) <= 0.5f) { 

                    _rb.velocity = Vector2.zero;

                }
                    

            }

        }

        return false;

    }

    void Shoot() {

        Vector2 targetLocation = GetClosestBillion();

        if(targetLocation != (Vector2)transform.position) {

            GameObject currBul = Instantiate(_bulletPrefab, _gunBarrelEnd.transform.position, Quaternion.identity);

            currBul.transform.localScale-=new Vector3(0.05f, 0.05f, 0f);

            currBul.SendMessage("SetTarget", new BulletData((targetLocation - (Vector2)transform.position).normalized * 50f, this.gameObject.GetComponent<SpriteRenderer>().color, 1));

        }

    }

    void AimBarrel() {

        Vector2 targetLocation = GetClosestBillion();

        if(targetLocation != (Vector2)transform.position) {

            _fireDir = targetLocation - (Vector2)this.gameObject.transform.position;

            _gunBarrelRotationPoint.transform.up = _fireDir;

        }

    }

    Vector2 GetClosestBillion() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, _billionDetectionRange);

        Vector2 closestBillionLoc = transform.position;

        foreach (Collider2D billion in colliders) {

            if(billion.gameObject.tag == "Billion" && billion.gameObject.GetComponent<SpriteRenderer>().color != this.gameObject.GetComponent<SpriteRenderer>().color) {
                Vector2 currentBillionLoc = billion.transform.position;

                if(closestBillionLoc == (Vector2)transform.position)
                    closestBillionLoc = currentBillionLoc;
                else if (Vector2.Distance(currentBillionLoc, this.gameObject.transform.position) < Vector2.Distance(closestBillionLoc, this.gameObject.transform.position))
                    closestBillionLoc = currentBillionLoc;

            }

        }

        return closestBillionLoc;

    }

}
