using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartMovement : MonoBehaviour {

    [Header("Base Variables")]
    [SerializeField]
    float _dartSpeed;
    float _dartDamage;
    Vector3 _movement;

    [Header("Detection Variables")]
    [SerializeField]
    bool _debugRays;
    [SerializeField]
    float _detectDist;
    [SerializeField]
    float _distBetweenRays;
    RaycastHit hit;

    DartTrap _trapThatFiredMe;
    bool _init = false;

	// Use this for initialization
	public void Init (float _damage, DartTrap trapThatFiredMe)
    {
        _movement = transform.forward * _dartSpeed * Time.deltaTime;
        _dartDamage = _damage;
        _trapThatFiredMe = trapThatFiredMe;

        _init = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(_init)
        {
            DartDetect();
            MoveDart();
        }
	}

    void MoveDart()
    {
        transform.position += _movement;
    }

    void DartDetect()
    {
        for (int i = -1; i < 2; i++)
        {
            Vector3 _rayPos = transform.position + (transform.right * _distBetweenRays * i);

            if(_debugRays)
            {
                Debug.DrawRay(_rayPos, transform.forward * _detectDist);
            }

            if (Physics.Raycast(_rayPos, transform.forward, out hit, _detectDist))
            {
                GameObject _thingHit = hit.collider.gameObject;

                if (_thingHit.GetComponent<PlayerController>())
                {
                    _thingHit.GetComponent<PlayerController>().TakeDamage(_dartDamage);
                }
                else if (!_thingHit.GetComponent<BaseEnemy>() && !_thingHit.GetComponent<BossEnemy>() && !_thingHit.GetComponent<DungeonMechanic>())
                {
                    _trapThatFiredMe.GetCurrDart = null;
                    Destroy(gameObject);
                }
            }
        }
        
    }
}
