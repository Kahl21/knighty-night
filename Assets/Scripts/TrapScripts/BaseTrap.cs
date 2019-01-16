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

    protected virtual void Start()
    {
        if(_independentTrap)
        {
            Init();
        }
    }

    public virtual void Init()
    {
        _playerRef = PlayerController.Instance;
        transform.parent = null;
    }

    public virtual void DisableTrap()
    {

    }

    public virtual void ResetTrap()
    {

    }
}
