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

    private enum BossOrientation
    {
        VERTICAL,
        HORIZONTAL
    }


    [Header("Dart Trap Variables")]
    [SerializeField]
    GameObject _dartsPrefab;
    GameObject _currDart;

    Vector3 _startPos;
    [SerializeField]
    float _spawnDelay;
    float _realSpawnDelay;
    [SerializeField]
    float _fireDelay;
    float _realFireDelay;
    float _currDelay;
    float _startDelay;
    
    DartState _myState = DartState.NONE;

    [Header("If in Boss Room")]
    [SerializeField]
    BossOrientation _myOrientationInBossRoom;
    float _moveSpeed;
    Vector3 _moveVector;
    float _XMin, _XMax, _ZMin, _ZMax;

    bool _possessed = false;
    float _possessedTime;
    float _currPossessTime;
    float _startPossessTime;
    TrapBossGlhost _bossRef;

    //Initilize function
    public override void Init()
    {
        base.Init();
        _startPos = transform.position;

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

        if (_possessed)
        {
            PossessionMoving();
        }
    }

    //creates a dart behind whatever wall the plate in on
    private void SpawnDart()
    {
        _currDelay = (Time.time - _startDelay) / _spawnDelay;

        Vector3 _spawnPos = transform.position;
        Quaternion _spawnRot = transform.rotation;

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _currDart = Instantiate<GameObject>(_dartsPrefab, _spawnPos, _spawnRot, this.transform);

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

            _currDart.transform.parent = null;
            _currDart.GetComponent<DartMovement>().Init(_trapDamage, this);

            _startDelay = Time.time;
            _myState = DartState.SPAWNING;
        }
    }

    public void BecomePossessed(TrapBossGlhost myboss, float possessDuration, float possessedFireDelay, float possessedSpawnDelay, float moveSpeed, float xmin, float xmax, float zmin, float zmax)
    {
        _bossRef = myboss;
        _possessedTime = possessDuration;
        _realFireDelay = possessedFireDelay;
        _realSpawnDelay = possessedSpawnDelay;
        _moveSpeed = moveSpeed;

        switch (_myOrientationInBossRoom)
        {
            case BossOrientation.VERTICAL:
                _ZMax = zmax;
                _ZMin = zmin;
                _moveVector = Vector3.up;
                break;
            case BossOrientation.HORIZONTAL:
                _XMax = xmax;
                _XMin = xmin;
                _moveVector = Vector3.right;
                break;
            default:
                break;
        }
        _possessed = true;
        _startPossessTime = Time.time;
    }

    void PossessionMoving()
    {
        _currPossessTime = (Time.time - _startPossessTime) / _possessedTime;

        if (_currPossessTime > 1)
        {
            _currPossessTime = 1;
            _possessed = false;
            _moveVector = Vector3.zero;
            _bossRef.IsPossessing = false;
        }

        switch (_myOrientationInBossRoom)
        {
            case BossOrientation.VERTICAL:
                if(transform.position.z > _ZMax)
                {
                    _moveVector = Vector3.down;
                }
                else if(transform.position.z < _ZMin)
                {
                    _moveVector = Vector3.up;
                }
                break;
            case BossOrientation.HORIZONTAL:
                if (transform.position.x > _XMax)
                {
                    _moveVector = Vector3.left;
                }
                else if (transform.position.x < _XMin)
                {
                    _moveVector = Vector3.right;
                }
                break;
            default:
                break;
        }

        transform.position += _moveVector * _moveSpeed * Time.deltaTime;
    }

    public override void DisableTrap()
    {
        if(_currDart != null)
        {
            Destroy(_currDart);
        }

        _possessed = false;
        _myState = DartState.ROOMDONE;
    }

    public override void ResetTrap()
    {
        if (_currDart != null)
        {
            Destroy(_currDart);
        }
        transform.position = _startPos;

        _myState = DartState.NONE;
    }

    public GameObject GetCurrDart { get { return _currDart; } set { _currDart = value; } }
}
