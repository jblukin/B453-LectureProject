using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _maxDistance = 5f;

    [SerializeField, Range(0, 5)] private float _explosionDuration;

    private Vector2 _target;

    private Vector2 _initialPosition;

    private Color _color;

    private GameObject _firingObject;

    private BulletData _myData;

    private bool _exploding;

    // Start is called before the first frame update
    void Start()
    {

        _initialPosition = transform.position;

        _exploding = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(!_exploding)
            transform.position = Vector2.MoveTowards(transform.position, _target, 5.0f * Time.deltaTime);

        if(Vector2.Distance(_initialPosition, transform.position) > _maxDistance)
            if(!_myData.isRocket)
                Destroy(this.gameObject);
            else {

                OnRocketBulletHit();

                Destroy(gameObject);

            }
        

    }

    void SetTarget(BulletData bD) {

        _target = bD.targetLoc;

        gameObject.GetComponent<SpriteRenderer>().color = bD.color;

        _color = bD.color;

        _firingObject = bD.firingObject;

        _myData = bD;

        if (bD.isRocket)
            _myData.damageAmount/=4f;

    }

    void OnRocketBulletHit() {


        _exploding = true;

        transform.localScale=new Vector3(0.75f, 0.75f, 0f);

        StartCoroutine("ExplosionTimer");

        Invoke("DealExplosionDamage", 0.0f);
        
    }

    void DealExplosionDamage() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, gameObject.GetComponent<Collider2D>().bounds.size.x/2);

        foreach(Collider2D collider in colliders) {

            if(collider.gameObject.CompareTag("Billion"))
                if(collider.gameObject is not null) collider.gameObject.SendMessage("TakeBulletDamage", _myData);
            else if(collider.gameObject.CompareTag("Base"))
                if(collider.gameObject is not null) collider.gameObject.SendMessage("TakeDamage", _myData);

        }

        if(_exploding)
            Invoke("DealExplosionDamage", 0.1f);

    }

    IEnumerator ExplosionTimer() {

        yield return new WaitForSeconds(_explosionDuration);
        Destroy(gameObject);

    }

    void OnTriggerEnter2D(Collider2D collider) {

        if(collider.gameObject.CompareTag("Billion")) {

            if(collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color != _color) {

                if(_myData.isRocket)
                    OnRocketBulletHit();
                else {   
                    
                    if(collider.gameObject is not null) collider.gameObject.SendMessage("TakeBulletDamage", _myData);

                    Destroy(gameObject);
                }

            }

        } else if(collider.gameObject.CompareTag("Base")) {

            if(_myData.isRocket)
                    OnRocketBulletHit();
                else {   
                    
                    if(collider.gameObject is not null) collider.gameObject.SendMessage("TakeDamage", _myData);

                    Destroy(gameObject);
                }

        } else if(collider.gameObject.CompareTag("Wall"))
            if(_myData.isRocket)
                    OnRocketBulletHit();
                else
                    Destroy(gameObject);

    }

}
