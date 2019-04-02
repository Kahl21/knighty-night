using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutBlock : ColoredBlock {

    BreakoutManager _breakoutRef;

    protected override void Awake()
    {
        
    }

    public virtual void Init(BreakoutManager myHolder, Color spawnColor)
    {
        GetComponent<MeshRenderer>().material.color = _myColor;
        _mySpotlight = transform.GetChild(0).GetComponent<Light>();
        _myColor = spawnColor;
        _mySpotlight.color = _myColor;

        _breakoutRef = myHolder;
    }

    public override void CorrectMatch()
    {
        gameObject.SetActive(false);
        _spawner.CheckForEnd();
    }
}
