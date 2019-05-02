using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutBlock : ColoredBlock {

    BreakoutManager _breakoutRef;
    bool _isDeathPillar = false;
    float _damageToPlayer;

    protected override void Awake()
    {
        
    }

    public virtual void Init(BreakoutManager myHolder, Color spawnColor, float damageToPlayer)
    {
        GetComponent<MeshRenderer>().material.color = _myColor;
        _mySpotlight = transform.GetChild(0).GetComponent<Light>();
        _myColor = spawnColor;
        _mySpotlight.color = _myColor;
        _breakoutRef = myHolder;
        _damageToPlayer = damageToPlayer;
    }

    public override void CorrectMatch()
    {
        gameObject.SetActive(false);
        _spawner.CheckForEnd();
    }

    public void DamagePlayer()
    {
        Debug.Log("damage Player");
        PlayerController.Instance.TakeDamage(_damageToPlayer);
    }

    public bool SetDeathPillar { get { return _isDeathPillar; } set { _isDeathPillar = value; } }
}
