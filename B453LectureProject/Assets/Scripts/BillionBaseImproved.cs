using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillionBaseImproved : MonoBehaviour
{
    [SerializeField] private GameObject _billionPrefab;

    [SerializeField] public GameObject flagPrefab;

    [SerializeField] private GameObject _barrelRotationPoint;

    [SerializeField] private GameObject _barrelEndPoint;

    [SerializeField] private GameObject _bulletPrefab;

    private GameObject _currentFlag = null;

    private Color _baseColor;

    private bool _canMoveFlag = false;

    private int _flagsPlaced = 0;

    [SerializeField] private float _baseDetectionRange = 20.0f;

    [SerializeField] private float _barrelRotationSpeed = 0.1f;

    private float _health;

    private float _maxHealth = 50.0f;

    private float _XPThreshold = 10.0f;

    private float _currentXP = 0.0f;

    [SerializeField] private GameObject _radialHealthBar;

    [SerializeField] private GameObject _radialXPBar;

    // Start is called before the first frame update
    void Start()
    {

        _baseColor = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color;

        _health = _maxHealth;

        InvokeRepeating("SpawnBillion", 0.0f, 2.0f);

        InvokeRepeating("Shoot", 1.0f, 5.0f);

    }

    // Update is called once per frame
    void Update()
    {
        spawnInitialFlags();

        AimBarrel();

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

            if(attempts > 10)
                break;
        }

        if(canSpawnHere) {

            GameObject billion = Instantiate(_billionPrefab, spawnPos, Quaternion.identity);

            billion.GetComponentInChildren<SpriteRenderer>().color = _baseColor;

            billion.SendMessage("SetHomeBase", this.gameObject);

        }

    }
    bool preventSpawnOverlap(Vector3 spawnPos) //Prevents Initial Spawn Overlap
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5);

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

    void spawnInitialFlags() 
    {

        if(_flagsPlaced < 2 && Input.GetKeyDown(KeyCode.Mouse0)) {

            if((Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1)) && _baseColor == Color.blue) {

                Instantiate(flagPrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 11f), Quaternion.identity, this.gameObject.transform);

                _flagsPlaced++;

            }
            else if((Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2)) && _baseColor == Color.green) {

                Instantiate(flagPrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 11f), Quaternion.identity, this.gameObject.transform);

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

                if(hit.collider.gameObject.tag == "Flag") {

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

    void Shoot() {

        Vector2 targetLocation = GetClosestBillion();

        if(targetLocation != Vector2.zero) {

            GameObject currBul = Instantiate(_bulletPrefab, _barrelEndPoint.transform.position, Quaternion.identity);

            currBul.transform.localScale+=new Vector3(0.15f, 0.15f, 0f);

            currBul.SendMessage("SetTarget", new BulletData((targetLocation - (Vector2)transform.position).normalized * 50f, _baseColor, 3,this.gameObject));

        }

    }

    void AimBarrel() {

        if(GetClosestBillion() != Vector2.zero) {

            Vector2 targetDir = GetClosestBillion() - (Vector2)transform.position;

            _barrelRotationPoint.transform.up = Vector2.MoveTowards(_barrelRotationPoint.transform.up, targetDir, _barrelRotationSpeed * Time.deltaTime);
        }

    }

    Vector2 GetClosestBillion() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _baseDetectionRange);

        Vector2 closestBillionLoc = Vector2.up * _baseDetectionRange;

        foreach (Collider2D billion in colliders) {

            if(billion.gameObject.CompareTag("Billion") && billion.gameObject.GetComponent<SpriteRenderer>().color != _baseColor) {

                Vector2 currentBillionLoc = billion.transform.position;

                if(Vector2.Distance(currentBillionLoc, this.gameObject.transform.position) <= Vector2.Distance(closestBillionLoc, this.gameObject.transform.position))
                    closestBillionLoc = currentBillionLoc;

            }

        }

        if(closestBillionLoc == (Vector2.up * _baseDetectionRange))
            closestBillionLoc = Vector2.zero;
        
        return closestBillionLoc;

    }

    void TakeDamage(BulletData bD) {

        _health-=bD.damageAmount;

        _radialHealthBar.GetComponent<Image>().fillAmount = (float)(_health / _maxHealth);

        if(_health <= 0)
            Destroy(this.gameObject);

    }

    void GainXP(int xpAmount) {

        _currentXP+=xpAmount;

        if(_currentXP >= _XPThreshold)
            _currentXP = 0.0f;
            //Add rank-up

        _radialXPBar.GetComponent<Image>().fillAmount = (float)(_currentXP / _XPThreshold);

    }

}

public class BulletData {

    public Vector2 targetLoc;
    public Color color;
    public int damageAmount;
    public GameObject firingObject;

    public BulletData(Vector2 target, Color c, int damage, GameObject objectShooting) {

        targetLoc = target;

        color = c;

        damageAmount = damage;

        firingObject = objectShooting;

    }

}
