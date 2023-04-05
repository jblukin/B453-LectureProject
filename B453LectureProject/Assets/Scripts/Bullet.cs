using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _maxDistance = 5f;

    private Vector2 _target;

    private Vector2 _initialPosition;

    private Color _color;

    private GameObject _firingObject;

    private BulletData _myData;

    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector2.MoveTowards(transform.position, _target, 5.0f * Time.deltaTime);

        if(Vector2.Distance(_initialPosition, transform.position) > _maxDistance)
            Destroy(this.gameObject);

    }

    void SetTarget(BulletData bD) {

        _target = bD.targetLoc;

        gameObject.GetComponent<SpriteRenderer>().color = bD.color;

        _color = bD.color;

        _firingObject = bD.firingObject;

        _myData = bD;

    }

    void OnTriggerEnter2D(Collider2D collider) {

        if(collider.gameObject.tag == "Billion" && collider.gameObject.GetComponent<SpriteRenderer>().color != _color) {

            collider.gameObject.SendMessage("TakeBulletDamage", _myData);

            Destroy(gameObject);

        } else if(collider.gameObject.CompareTag("Base") && collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color != _color) {

            collider.gameObject.SendMessage("TakeDamage", _myData);
            
            Destroy(gameObject);

        }

    }

}
