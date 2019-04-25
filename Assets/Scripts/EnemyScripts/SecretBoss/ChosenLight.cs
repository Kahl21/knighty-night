using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosenLight : MonoBehaviour {

    float _fallSpeed;
    float _damage;
    [SerializeField]
    float _detectionDistance;
    [SerializeField]
    bool _debug;
    RaycastHit hit;
    bool _init;

	// Use this for initialization
	public void Init (float damage, float speed) {
        _damage = damage;
        _fallSpeed = speed;
        _init = true;
	}
	
	// Update is called once per frame
	void Update () {
        if(_init)
        {
            Fall();
        }
	}

    void Fall()
    {
        if(_debug)
        {
            Debug.DrawRay(transform.position, transform.position + Vector3.down * _detectionDistance);
        }

        if (Physics.Raycast(transform.position, Vector3.down, out hit, _detectionDistance))
        {
            if (hit.collider.GetComponent<PlayerController>())
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(_damage);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
    }
}
