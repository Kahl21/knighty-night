using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartTrap : BaseTrap {

    private enum DartState
    {
        NONE,
        SPAWNING,
        FIRING,
        ROOMDONE
    }

    [Header("Dart Trap Variables")]
    [SerializeField]
    GameObject _dartsPrefab;
    GameObject _currDart;

    Vector3 _spawnPos;
    Quaternion _spawnRot;
    [SerializeField]
    float _spawnDelay;
    [SerializeField]
    float _fireDelay;
    float _currDelay;
    float _startDelay;

    DartState _myState = DartState.NONE;
    
    public override void Init()
    {
        base.Init();
        _spawnPos = transform.position;
        _spawnRot = transform.rotation;

        _startDelay = Time.time;
        _myState = DartState.SPAWNING;
    }

    // Update is called once per frame
    void Update () {
        switch (_myState)
        {
            case DartState.NONE:
                break;
            case DartState.SPAWNING:
                SpawnDart();
                break;
            case DartState.FIRING:
                FireDart();
                break;
            case DartState.ROOMDONE:
                break;
            default:
                break;
        }
    }

    private void SpawnDart()
    {
        _currDelay = (Time.time - _startDelay) / _spawnDelay;

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _currDart = Instantiate<GameObject>(_dartsPrefab, _spawnPos, _spawnRot);

            _startDelay = Time.time;
            _myState = DartState.FIRING;
        }
    }

    private void FireDart()
    {
        _currDelay = (Time.time - _startDelay) / _spawnDelay;

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _currDart.GetComponent<DartMovement>().Init(_trapDamage);

            _startDelay = Time.time;
            _myState = DartState.SPAWNING;
        }
    }

    public override void DisableTrap()
    {
        if(_myState != DartState.FIRING)
        {
            Destroy(_currDart);
        }

        _myState = DartState.ROOMDONE;
    }

    public override void ResetTrap()
    {
        if (_currDart != null)
        {
            Destroy(_currDart);
        }

        _myState = DartState.NONE;
    }
}
