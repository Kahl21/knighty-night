using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTrap : MonoBehaviour {

    [Header("Base Variables")]
    [SerializeField]
    protected float _trapDamage;
    [SerializeField]
    protected bool _independentTrap;

    protected PlayerController _playerRef;

    //start Function
    //checks if the trap is independent of a room
    //calls Init based on whether or not independent
    protected virtual void Start()
    {
        if(_independentTrap)
        {
            Init();
        }
    }

    //init function
    //starts the trap
    public virtual void Init()
    {
        _playerRef = PlayerController.Instance;
        transform.parent = null;
    }

    //Stops trap from working
    public virtual void DisableTrap()
    {

    }

    //reset function
    public virtual void ResetTrap()
    {

    }
}
