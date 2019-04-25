using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleTrigger : MonoBehaviour {

    ParticleSystem _ps;
    PlayerController _playerRef;

	// Use this for initialization
	void Start ()
    {
        _ps = GetComponent<ParticleSystem>();
        _playerRef = PlayerController.Instance;

        //_ps.trigger.radiusScale = 0.5f;

        _ps.trigger.SetCollider(0, _playerRef.GetComponent<Collider>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
    private void OnParticleTrigger()
    {
        _playerRef.TakeDamage(transform.parent.GetComponent<BossFireStatueTrap>().GetFireDamage);
    }
    */
}
