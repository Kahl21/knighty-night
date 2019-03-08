using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mechanic
{
    NONE,
    SWARM,
    CHASE,
    COLOR,
    TRAP,
    BOSS,
    HEAL,
    BREAKOUT
}

public class DungeonMechanic : MonoBehaviour {

    float _XMin;
    float _XMax;
    float _ZMin;
    float _ZMax;
    List<BaseEnemy> _enemyList = new List<BaseEnemy>();

    [Header("Mandatory Variables")]
    [SerializeField]
    Mechanic _roomMechanic;
    [SerializeField]
    List<DoorMovement> _doors;
    [SerializeField]
    DungeonMechanic _secondaryMechanic;

    [Header("If Room Has Enemies")]
    [SerializeField]
    GameObject _enemyPreFab;

    [Header("If Room Has Traps")]
    [SerializeField]
    List<BaseTrap> _trapsInRoom;

    [Header("If Swarm Room Variables")]
    [SerializeField]
    float _startSpawnDelay;
    [SerializeField]
    float _delayBetweenSpawns;
    float _startSpawningTime;
    float _currSpawningTime;
    [SerializeField]
    float _howManyGlhostsToKill;
    float _glhostsSpawned = 0;
    float _glhostsKilled = 0;
    bool _roomStarted = false;
    bool _spawning = false;

    [Header("If Chase Room Variables")]
    [SerializeField]
    GameObject _spawnPoint;
    [SerializeField]
    float _howManyGlhostsToSpawn;
    [SerializeField]
    List<MazeCheckpoint> _mazeCheckpoints;
    [SerializeField]
    bool _debugChangeArea;
    [SerializeField]
    float _ghlostChangeDistance;
    [SerializeField]
    GameObject _chaseChangePoint;

    [Header("If Color Room Variables")]
    [SerializeField]
    List<ColoredBlock> _coloredBlocks;
    [SerializeField]
    List<GameObject> _spawnPoints;
    List<GameObject> _offsetSpawnPointsHolder = new List<GameObject>();

    [Header("If Trap Room Variables")]
    [SerializeField]
    List<TrapLever> _LeversToEndRoom;

    [Header("If Boss Room Variables")]
    BossEnemy _BigBad;

    [Header("If Heal Room Variables")]
    [SerializeField]
    GameObject _healObject;

    [Header("If Breakout Room")]
    [SerializeField]
    GameObject _puckSpawnPoint;
    BreakoutManager _breakRef;

    GameManager _managerRef;
    BoxCollider _myCollider;

    [Header("Number Of The Next Checkpoint")]
    public float nextCheckpoint;

    //If Breakout Room

    private void Awake()
    {
        _managerRef = GameManager.Instance;
        _managerRef.SetGameReset += RestartRoom;
        _myCollider = transform.GetComponent<BoxCollider>();
    }

    public void Init()
    {
        AudioManager.instance.GateClose();

        _myCollider.enabled = false;

        if(_doors.Count > 0 && _roomMechanic != Mechanic.CHASE)
        {
            for (int i = 0; i < _doors.Count; i++)
            {
                _doors[i].Init();
            }
        }

        if (_trapsInRoom.Count > 0)
        {
            for (int i = 0; i < _trapsInRoom.Count; i++)
            {
                _trapsInRoom[i].Init();
            }
        }

        switch (_roomMechanic)
        {
            case Mechanic.NONE:
                break;
            case Mechanic.SWARM:
                //Debug.Log("spawningStarted");
                _XMin = (transform.position.x - (transform.localScale.x * 5f));
                _ZMin = (transform.position.z - (transform.localScale.z * 5f));
                _XMax = (transform.position.x + (transform.localScale.x * 5f));
                _ZMax = (transform.position.z + (transform.localScale.z * 5f));

                //Debug.Log("" + _XMax + ", " + _XMin + ", " + _ZMax + ", " + _ZMin + ", ");
                _glhostsSpawned = 0;
                _roomStarted = true;
                break;
            case Mechanic.CHASE:
                for (int i = 0; i < _howManyGlhostsToSpawn; i++)
                {
                    SpawnEnemy(0);
                }

                for (int i = 0; i < _mazeCheckpoints.Count; i++)
                {
                    _mazeCheckpoints[i].Init(this, _enemyList, _mazeCheckpoints);
                }
                break;
            case Mechanic.COLOR:
                for (int i = 0; i < _coloredBlocks.Count; i++)
                {
                    _coloredBlocks[i].SetSpawner = this;
                    SpawnEnemy(i);
                }
                break;
            case Mechanic.TRAP:
                for (int i = 0; i < _LeversToEndRoom.Count; i++)
                {
                    _LeversToEndRoom[i].Init(this);
                }
                break;
            case Mechanic.BOSS:
                _BigBad = _enemyPreFab.GetComponent<BossEnemy>();
                _BigBad.SetMyRoom = this;
                _managerRef.GetPlayer.GoingToIntroCutscene(_BigBad);
                break;
            case Mechanic.HEAL:
                break;
            case Mechanic.BREAKOUT:
                _breakRef = GetComponent<BreakoutManager>();
                _breakRef.Init();
                SpawnEnemy(0);
                break;
            default:
                break;
        }

        if (_secondaryMechanic != null)
        {
            if (_secondaryMechanic.GetMechanic != Mechanic.CHASE)
            {
                _secondaryMechanic.Init();
            }
            else
            {
                for (int i = 0; i < _secondaryMechanic.GetDoors.Count; i++)
                {
                    _secondaryMechanic.GetDoors[i].Init();
                }
            }
        }
       
    }

    private void Update()
    {
        if(_roomStarted)
        {
            switch (_roomMechanic)
            {
                case Mechanic.SWARM:
                    if (_glhostsSpawned < _howManyGlhostsToKill)
                    {
                        if (_spawning)
                        {
                            SpawningDelay();
                        }
                        else
                        {
                            StartDelay();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(_debugChangeArea)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_chaseChangePoint.transform.position, _ghlostChangeDistance);
        }
    }

    void SpawningDelay()
    {
        _currSpawningTime = (Time.time - _startSpawningTime) / _delayBetweenSpawns;

        if(_currSpawningTime >= 1)
        {
            _currSpawningTime = 1;

            SpawnEnemy(0);
            _startSpawningTime = Time.time;
        }
        
    }

    void StartDelay()
    {
        _currSpawningTime = (Time.time - _startSpawningTime) / _startSpawnDelay;

        if (_currSpawningTime >= 1)
        {
            _currSpawningTime = 1;

            _spawning = true;
            _startSpawningTime = Time.time;
        }
    }

    private void SpawnEnemy(int listNum)
    {
        Vector3 spawnPos = transform.position;

        switch (_roomMechanic)
        {
            case Mechanic.NONE:
                Debug.Log("No Mechanic");
                break;
            case Mechanic.SWARM:
                float XorZ = Random.Range(-1f, 1f);
                //Debug.Log("FirstCheck " + XorZ);
                if (XorZ > 0)
                {
                    XorZ = Random.Range(-1f, 1f);
                    //Debug.Log("SecondCheck " + XorZ);
                    if (XorZ > 0)
                    {
                        spawnPos.x = _XMax;

                    }
                    else
                    {
                        spawnPos.x = _XMin;
                    }
                    spawnPos.z = Random.Range(_ZMin, _ZMax);
                }
                else
                {
                    //Debug.Log("SecondCheck " + XorZ);
                    XorZ = Random.Range(-1f, 1f);
                    if (XorZ > 0)
                    {
                        spawnPos.z = _ZMax;

                    }
                    else
                    {
                        spawnPos.z = _ZMin;
                    }
                    spawnPos.x = Random.Range(_XMin, _XMax);
                }
                break;
            case Mechanic.CHASE:
                spawnPos = _spawnPoint.transform.position;
                break;
            case Mechanic.COLOR:
                int spawnPosNum = Random.Range(0, _spawnPoints.Count);
                spawnPos = _spawnPoints[spawnPosNum].transform.position;
                _offsetSpawnPointsHolder.Add(_spawnPoints[spawnPosNum]);
                _spawnPoints.Remove(_spawnPoints[spawnPosNum]);
                break;
            case Mechanic.BREAKOUT:
                spawnPos = _puckSpawnPoint.transform.position;
                break;
            default:
                break;
        }

        spawnPos.y = 0;
        //Debug.Log(spawnPos);
        GameObject newEnemy = Instantiate<GameObject>(_enemyPreFab, spawnPos, _enemyPreFab.transform.rotation);

        switch (_roomMechanic)
        {
            case Mechanic.CHASE:
                newEnemy.GetComponent<GraveyardGlhost>().SetEndPoint = _chaseChangePoint;
                break;
            case Mechanic.COLOR:
                newEnemy.GetComponent<BaseEnemy>().SetColor = _coloredBlocks[listNum].GetColor;
                newEnemy.GetComponent<BaseEnemy>().SetPillar = _coloredBlocks[listNum].gameObject;
                newEnemy.transform.GetChild(4).GetComponent<Light>().color = _coloredBlocks[listNum].GetColor;
                break;
            default:
                break;
        }

        newEnemy.GetComponent<BaseEnemy>().Init(this, _roomMechanic);
        _enemyList.Add(newEnemy.GetComponent<BaseEnemy>());
        _glhostsSpawned++;
    }

    public void CheckForEnd()
    {
        switch (_roomMechanic)
        {
            case Mechanic.NONE:
                break;
            case Mechanic.SWARM:
                _glhostsKilled++;
                if(_glhostsKilled >= _howManyGlhostsToKill)
                {
                    EndAll();
                }
                break;
            case Mechanic.CHASE:
                EndAll();
                break;
            case Mechanic.COLOR:
                for (int i = 0; i < _coloredBlocks.Count; i++)
                {
                    if(_coloredBlocks[i].gameObject.activeInHierarchy)
                    {
                        return;
                    }
                }
                EndAll();
                break;
            case Mechanic.TRAP:
                for (int i = 0; i < _LeversToEndRoom.Count; i++)
                {
                    if(!_LeversToEndRoom[i].CheckIfActivated)
                    {
                        return;
                    }
                }
                EndAll();
                break;
            case Mechanic.BOSS:
                EndAll();
                break;
            default:
                break;
        }
    }

    public void EndAll()
    {
        _roomStarted = false;
        _spawning = false;

        if (_roomMechanic != Mechanic.CHASE)
        {
            AudioManager.instance.GateOpen();

            if (_doors.Count > 0)
            {
                for (int i = 0; i < _doors.Count; i++)
                {
                    _doors[i].RoomDone();
                }
            }
        }

        if (_secondaryMechanic != null)
        {
            _secondaryMechanic.EndAll();
        }

        if(_enemyList.Count > 0)
        {
            for (int i = _enemyList.Count - 1; i >= 0; i--)
            {
                _enemyList[i].GetComponent<BaseEnemy>().Stop();
            }
        }

        if(_trapsInRoom.Count > 0)
        {
            for (int i = 0; i < _trapsInRoom.Count; i++)
            {
                _trapsInRoom[i].DisableTrap();
            }
        }

        _glhostsKilled = 0;
        _glhostsSpawned = 0;

    }

    public void AddEnemy(BaseEnemy _enemyToAdd)
    {
        _enemyList.Add(_enemyToAdd);
    }

    public void RemoveMe(BaseEnemy _enemyToRemove)
    {
        _enemyList.Remove(_enemyToRemove);
    }

    public void RemoveBlock(ColoredBlock _colorBlockToRemove)
    {
        _coloredBlocks.Remove(_colorBlockToRemove);
    }

    private void RestartRoom()
    {
        _myCollider.enabled = true;

        switch (_roomMechanic)
        {
            case Mechanic.NONE:
                break;
            case Mechanic.SWARM:
                EndAll();
                break;
            case Mechanic.CHASE:
                if (_doors.Count > 0) 
                {
                    for (int i = 0; i < _doors.Count; i++)
                    {
                        _doors[i].RoomDone();
                    }
                }
                for (int i = 0; i < _mazeCheckpoints.Count; i++)
                {
                    _mazeCheckpoints[i].MyReset();
                }
                EndAll();
                break;
            case Mechanic.COLOR:
                EndAll();
                for (int i = 0; i < _coloredBlocks.Count; i++)
                {
                    _coloredBlocks[i].gameObject.SetActive(true);
                }

                for (int i = _offsetSpawnPointsHolder.Count-1; i >= 0; i--)
                {
                    _spawnPoints.Add(_offsetSpawnPointsHolder[i]);
                    _offsetSpawnPointsHolder.Remove(_offsetSpawnPointsHolder[i]);
                }
                break;
            case Mechanic.TRAP:
                EndAll();
                for (int i = 0; i < _LeversToEndRoom.Count; i++)
                {
                    //Debug.Log("lever reseting");
                    _LeversToEndRoom[i].ResetLever();
                }

                for (int i = 0; i < _trapsInRoom.Count; i++)
                {
                    _trapsInRoom[i].ResetTrap();
                }
                break;
            case Mechanic.BOSS:
                _BigBad = _enemyPreFab.GetComponent<BossEnemy>();
                EndAll();
                _BigBad.MyReset();
                break;
            case Mechanic.HEAL:
                _healObject.SetActive(true);
                _healObject.GetComponent<HealingGrace>().HealReset();
                break;
            default:
                break;
        }
    }

    public void DisableRoom()
    {
        _myCollider.enabled = false;
        EndAll();
    }

    public BossEnemy GetBigBoy { get { return _BigBad; } }
    public Mechanic GetMechanic { get { return _roomMechanic; } }
    public List<DoorMovement> GetDoors { get { return _doors; } }
    public List<BaseEnemy> GetCurrEnemyList { get { return _enemyList; } }
    public List<BaseTrap> GetCurrTrapList { get { return _trapsInRoom; } }
    public float GetChangeDistance { get { return _ghlostChangeDistance; } }
}