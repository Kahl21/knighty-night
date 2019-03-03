using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBossTrap : BaseTrap {

    /*
    / 
    /   CREATED BECAUSE I WAS TOO LAZY TO REWORK ANYTHING     
    /
    */

    List<BaseTrap> _trapsOnThis;
        
    protected override void Start()
    {
        _trapsOnThis = new List<BaseTrap>();
        for (int i = 0; i < transform.childCount; i++)
        {
            _trapsOnThis.Add(transform.GetChild(i).GetComponent<BaseTrap>());
        }
    }

    //init function
    //starts the trap
    public override void Init()
    {
        
    }

    //Stops trap from working
    public override void DisableTrap()
    {
        for (int i = 0; i < _trapsOnThis.Count; i++)
        {
            _trapsOnThis[i].DisableTrap();
        }
    }

    //reset function
    public override void ResetTrap()
    {
        for (int i = 0; i < _trapsOnThis.Count; i++)
        {
            _trapsOnThis[i].ResetTrap();
        }
    }
}
