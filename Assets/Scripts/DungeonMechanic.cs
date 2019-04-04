using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mechanic                                        //Enum for mechanic of the room
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

    
    

    [Header("Mandatory Variables")]
    [SerializeField]
    Mechanic _roomMechanic;                                 //mechanic reference
    [SerializeField]
    List<DoorMovement> _doors;                              //List of all doors associated with this room
    [SerializeField]
    DungeonMechanic _secondaryMechanic;                     //(Optional) Access to another room that acts as a secondary mechanic
    List<BaseEnemy> _enemyList = new List<BaseEnemy>();     //List of all enemies currently in the room

    [Header("If Room Has Enemies")]
    [SerializeField]
    GameObject _enemyPreFab;                                //the main enemy type inside of the room

    [Header("If Room Has Traps")]
    [SerializeField]
    List<BaseTrap> _trapsInRoom;                            //(Optional) List of all traps in the room

    [Header("If Swarm Room Variables")]
    [SerializeField]
    float _startSpawnDelay;                                 //delay for spawning right when the room is activated
    [SerializeField]
    float _delayBetweenSpawns;                              //delay between spawning ghlosts
    float _startSpawningTime;                               //starting time.time once spawning begins
    float _currSpawningTime;                                //keeps track of when spawning should start or not due to math
    [SerializeField]
    float _howManyGlhostsToKill;                            //Howw many ghlosts to die before the room is complete
    float _glhostsSpawned = 0;                              //how many ghlots have been spawned
    float _glhostsKilled = 0;                               //how many ghlosts have been killed
    bool _roomStarted = false;                              //has the room been activated yet?
    bool _spawning = false;                                 //is the room allowed to spawn ghlosts?
    //for swarm rooms
    float _XMin;                                            //left side of spawn area          
    float _XMax;                                            //right side of spawn area
    float _ZMin;                                            //back of the spawn area
    float _ZMax;                                            //front of the spawn area

    [Header("If Chase Room Variables")]
    [SerializeField]
    GameObject _spawnPoint;                                 //gameobject reference of where to start spawning ghlosts
    [SerializeField]
    float _howManyGlhostsToSpawn;                           //how many chase ghlosts to spawn
    [SerializeField]
    List<MazeCheckpoint> _mazeCheckpoints;                  //reference to all checkpoints in chase room
    [SerializeField]
    bool _debugChangeArea;                                  //gizmo debug for level design for when the ghlost changes
    [SerializeField]
    float _ghlostChangeDistance;                            //how far until the ghlosts switch mechanics
    [SerializeField]
    GameObject _chaseChangePoint;                           //gameobject reference to where the ghlosts should start to change (the ending room)

    [Header("If Color Room Variables")]
    [SerializeField]
    List<ColoredBlock> _coloredBlocks;                      //list reference of the color pillars in room
    [SerializeField]
    List<GameObject> _spawnPoints;                          //list of points where to spawn all of the ghlosts
    List<GameObject> _offsetSpawnPointsHolder = new List<GameObject>();         //duplicate list of points to keep track of points that have already been used

    [Header("If Trap Room Variables")]
    [SerializeField]
    List<TrapLever> _LeversToEndRoom;                       //list to keep track of all levers in the room

    [Header("If Boss Room Variables")]
    BossEnemy _BigBad;                                      //boss reference

    [Header("If Heal Room Variables")]
    [SerializeField]
    GameObject _healObject;                                 //heal aura reference

    [Header("If Breakout Room")]
    [SerializeField]
    GameObject _puckSpawnPoint;                             //gameobject on where to spawn the puck
    BreakoutManager _breakRef;                              //reference to the breakout manager

    GameManager _managerRef;                                //refernce to the main game manager
    BoxCollider _myCollider;                                //rooms collider

    [Header("Number Of The Next Checkpoint")]
    public float nextCheckpoint;                            //if this room is a checkpoint, which one is it?

    //awake happens when the scene loads in
    private void Awake()
    {
        _managerRef = GameManager.Instance;                 //grab game manager singleton for reference
        _managerRef.SetGameReset += RestartRoom;            //add my reset function to the reset game delegate
        _myCollider = transform.GetComponent<BoxCollider>();        //fills reference the collider on my person
    }

    //called when the player walks into the room
    public void Init()
    {
        AudioManager.instance.GateClose();                  //play gate closing sound

        _myCollider.enabled = false;                        //turn off collider

        if(_doors.Count > 0 && _roomMechanic != Mechanic.CHASE)     //if there are any doors and the mechanic is not a chase room
        {
            for (int i = 0; i < _doors.Count; i++)
            {
                _doors[i].Init();                           //initalize all doors
            }
        }

        if (_trapsInRoom.Count > 0)                         //if there are any traps in the room
        {
            for (int i = 0; i < _trapsInRoom.Count; i++)   
            {
                _trapsInRoom[i].Init();                     //initialize all of the traps 
            }
        }

        switch (_roomMechanic)
        {
            case Mechanic.NONE:
                break;
            case Mechanic.SWARM:                                                    //if swarm room
                
                //Debug.Log("spawningStarted");

                //localscale * 5f = roughly one unit

                _XMin = (transform.position.x - (transform.localScale.x * 5f));     //set left wall of spawn
                _ZMin = (transform.position.z - (transform.localScale.z * 5f));     //set back wall of spawn
                _XMax = (transform.position.x + (transform.localScale.x * 5f));     //set right wall of spawn
                _ZMax = (transform.position.z + (transform.localScale.z * 5f));     //set back wall of spawn

                //Debug.Log("" + _XMax + ", " + _XMin + ", " + _ZMax + ", " + _ZMin + ", ");
                _glhostsSpawned = 0;                                                //set base ghlosts spawned 
                _roomStarted = true;                                                //room is ready to operate
                break;

            case Mechanic.CHASE:                                        //if chase room

                for (int i = 0; i < _howManyGlhostsToSpawn; i++)        //check how many chase ghlosts to spawn
                {
                    SpawnEnemy(0);                                      //spawn an enemy
                }

                for (int i = 0; i < _mazeCheckpoints.Count; i++)
                {
                    _mazeCheckpoints[i].Init(this, _enemyList, _mazeCheckpoints);       //activate all maze checkpoints
                }
                break;

            case Mechanic.COLOR:                                        //if color room

                for (int i = 0; i < _coloredBlocks.Count; i++)
                {
                    _coloredBlocks[i].SetSpawner = this;                //activate all pillars
                    SpawnEnemy(i);                                      //spawn an enemy
                }
                break;

            case Mechanic.TRAP:                                         //if trap room

                for (int i = 0; i < _LeversToEndRoom.Count; i++)
                {
                    _LeversToEndRoom[i].Init(this);                     //activate all levers
                }
                break;

            case Mechanic.BOSS:

                _BigBad = _enemyPreFab.GetComponent<BossEnemy>();       //set boss reference from enemy reference
                _BigBad.SetMyRoom = this;                               //set boss's room to this            
                _managerRef.GetPlayer.GoingToIntroCutscene(_BigBad);    //get the player and put him in cutscene mode, also passing him the reference to the current boss
                break;

            case Mechanic.HEAL:                                         //if heal room
                break;
            case Mechanic.BREAKOUT:                                     //if breakout room

                _breakRef = GetComponent<BreakoutManager>();            //set breakout reference
                _breakRef.Init();                                       //initialize breakout room
                SpawnEnemy(0);                                          //spawn puck
                break;

            default:
                break;
        }

        if (_secondaryMechanic != null)                                 //if this room has a secondary mechanic
        {
            if (_secondaryMechanic.GetMechanic != Mechanic.CHASE)       //if the room is not a chase room
            {
                _secondaryMechanic.Init();                              //initialize secondary room 
            }
            else                                                        //else
            {
                if(_doors.Count > 0)
                {
                    for (int i = 0; i < _secondaryMechanic.GetDoors.Count; i++)
                    {
                        _secondaryMechanic.GetDoors[i].Init();              //initialize the doors on the secondary rooms doors
                    }
                }
                
            }
        }
       
    }

    private void Update()
    {
        if(_roomStarted)                                            //if the room is activated
        {
            switch (_roomMechanic)                                      
            {
                case Mechanic.SWARM:                                //if a swarm room
                    if (_glhostsSpawned < _howManyGlhostsToKill)    //if there are still more ghlosts to spawn
                    {
                        if (_spawning)                              //if spawning
                        {
                            SpawningDelay();                        //delay then spawn 
                        }
                        else                                        //else
                        {
                            StartDelay();                           //start the delay for spawning
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    //gizmo for drawing the area for when chase ghlosts change into another mechanic
    private void OnDrawGizmos()         
    {
        if(_debugChangeArea)
        {
            Gizmos.color = Color.green;                                                             //set gizmo color to green
            Gizmos.DrawWireSphere(_chaseChangePoint.transform.position, _ghlostChangeDistance);     //draw area based on _ghlostChangeDistance
        }
    }

    //delay inbetween spawns
    void SpawningDelay()
    {
        _currSpawningTime = (Time.time - _startSpawningTime) / _delayBetweenSpawns;     //math to see if delay is over

        if(_currSpawningTime >= 1)                      //if delay is over
        {
            _currSpawningTime = 1;

            SpawnEnemy(0);                              //spawn an enemy
            _startSpawningTime = Time.time;             //reset starting delay time
        }
        
    }

    //starting delay to spawn rooms
    void StartDelay()
    {
        _currSpawningTime = (Time.time - _startSpawningTime) / _startSpawnDelay;     //math to see if delay is over

        if (_currSpawningTime >= 1)
        {
            _currSpawningTime = 1;

            _spawning = true;                           //room now allowed to spawn
            _startSpawningTime = Time.time;             //reset starting delay time
        }
    }

    private void SpawnEnemy(int listNum)
    {
        Vector3 spawnPos = transform.position;                      //base spawn point before extra math

        switch (_roomMechanic)
        {
            case Mechanic.NONE:
                Debug.Log("No Mechanic");
                break;
            case Mechanic.SWARM:                                    //if a swarm room

                float XorZ = Random.Range(-1f, 1f);                 //random check to see if spawn is front/back edge or left/right edge
                //Debug.Log("FirstCheck " + XorZ);
                if (XorZ > 0)                                       //if check is greater than 0
                {
                    XorZ = Random.Range(-1f, 1f);                   //check again to see if left or right
                    //Debug.Log("SecondCheck " + XorZ);
                    if (XorZ > 0)                                   //if check is greater than 0
                    {
                        spawnPos.x = _XMax;                         //start pos is on the right edge
                    }
                    else                                            //if check is less than 0
                    {
                        spawnPos.x = _XMin;                         //start pos is on the left edge
                    }
                    spawnPos.z = Random.Range(_ZMin, _ZMax);        //randomize how far up/down on the left/right edge
                }
                else
                {
                    //Debug.Log("SecondCheck " + XorZ);
                    XorZ = Random.Range(-1f, 1f);                   //check again to see if left or right
                    if (XorZ > 0)                                   //if check is greater than 0
                    {
                        spawnPos.z = _ZMax;                         //start pos is on the front edge
                    }
                    else                                            //if check is less than 0
                    {
                        spawnPos.z = _ZMin;                         //start pos is on the back edge
                    }
                    spawnPos.x = Random.Range(_XMin, _XMax);        //randomize how far up/down on the left/right edge
                }
                break;

            case Mechanic.CHASE:                                    //if chase room

                spawnPos = _spawnPoint.transform.position;          //change spawn point to the chase spawn point
                break;

            case Mechanic.COLOR:

                int spawnPosNum = Random.Range(0, _spawnPoints.Count);          //choose a random spawnpoint
                spawnPos = _spawnPoints[spawnPosNum].transform.position;        //change spawn pos to that point
                _offsetSpawnPointsHolder.Add(_spawnPoints[spawnPosNum]);        //add the used point into the offset list
                _spawnPoints.Remove(_spawnPoints[spawnPosNum]);                 //remoce that used point from the main list
                break;

            case Mechanic.BREAKOUT:

                spawnPos = _puckSpawnPoint.transform.position;                  //change spawn point to the puck spawn point           
                break;

            default:
                break;
        }

        spawnPos.y = 0;                                                         //set y pos of spawn to the floor (just in case)
        //Debug.Log(spawnPos);
        GameObject newEnemy = Instantiate<GameObject>(_enemyPreFab, spawnPos, _enemyPreFab.transform.rotation);     //spawn the new enemy and set spawned ghlost to reference

        switch (_roomMechanic)
        {
            case Mechanic.CHASE:                                                                    //if chase room

                newEnemy.GetComponent<GraveyardGlhost>().SetEndPoint = _chaseChangePoint;           //grab graveyard ghlost script and set their end point
                break;

            case Mechanic.COLOR:                                                                    //if color room

                newEnemy.GetComponent<BaseEnemy>().SetColor = _coloredBlocks[listNum].GetColor;                 //set the color ghlosts starting color
                newEnemy.GetComponent<BaseEnemy>().SetPillar = _coloredBlocks[listNum].gameObject;              //set the pillar associated with that ghlosts color    
                newEnemy.transform.GetChild(0).GetComponent<Light>().color = _coloredBlocks[listNum].GetColor;  //ge the light of the color ghlost and change its color to the color of the pillar
                break;

            default:
                break;
        }

        newEnemy.GetComponent<BaseEnemy>().Init(this, _roomMechanic);           //initialize the spawned enemy
        _enemyList.Add(newEnemy.GetComponent<BaseEnemy>());                     //add spawned enemy to the reference list
        _glhostsSpawned++;                                                      //increment ghlosts spawned
    }

    //check to see if the room should end
    public void CheckForEnd()
    {
        switch (_roomMechanic)
        {
            case Mechanic.NONE:
                break;
            case Mechanic.SWARM:

                _glhostsKilled++;                               //a ghlost has been killed
                if(_glhostsKilled >= _howManyGlhostsToKill)     //if enough ghlosts have been killed
                {
                    EndAll();                                   //end the room
                }
                break;

            case Mechanic.CHASE:

                EndAll();                                       //end the room
                break;

            case Mechanic.COLOR:

                for (int i = 0; i < _coloredBlocks.Count; i++)
                {
                    if(_coloredBlocks[i].gameObject.activeInHierarchy)  //if any color pillar is not turned off
                    {
                        return;                                 //dont end the room, return
                    }
                }
                EndAll();                                       //end the room
                break;

            case Mechanic.TRAP:

                for (int i = 0; i < _LeversToEndRoom.Count; i++)
                {
                    if(!_LeversToEndRoom[i].CheckIfActivated)   //if a lever has not been activated
                    {
                        return;                                 //dont end room, return
                    }
                }
                EndAll();                                       //end the room
                break;

            case Mechanic.BOSS:

                EndAll();                                       //end the room
                break;

            default:
                break;
        }
    }


    //ends the room
    public void EndAll()
    {
        _roomStarted = false;                                           //stop room working
        _spawning = false;                                              //stop room spawning

        if (_roomMechanic != Mechanic.CHASE)                            //if the mechanic is not a chase room
        {
            AudioManager.instance.GateOpen();                           //play sound for opening a gate

            if (_doors.Count > 0)                                       //if there are any doors 
            {
                for (int i = 0; i < _doors.Count; i++)
                {
                    _doors[i].RoomDone();                               //raise the gates
                }
            }
        }

        if (_secondaryMechanic != null)                                 //if there is a secondary room
        {
            _secondaryMechanic.EndAll();                                //end that room
        }

        if(_enemyList.Count > 0)                                        //if the enemy list is not empty
        {
            for (int i = _enemyList.Count - 1; i >= 0; i--)
            {
                _enemyList[i].GetComponent<BaseEnemy>().Stop();         //stop all of the ghlosts in the list
            }
        }

        if(_trapsInRoom.Count > 0)                                      //if there are any traps in the room
        {
            try
            {
                if (_BigBad.GetComponent<TrapBossGlhost>())
                {
                    for (int i = 0; i < _trapsInRoom.Count; i++)
                    {
                        _trapsInRoom[i].ResetTrap();                          //stop all of the traps
                    }
                }
            }
            catch
            {
                for (int i = 0; i < _trapsInRoom.Count; i++)
                {
                    _trapsInRoom[i].DisableTrap();                          //stop all of the traps
                }
            }
        }

        _glhostsKilled = 0;                                             //resets killed ghlosts
        _glhostsSpawned = 0;                                            //resets spawned ghlosts

    }

    //adds an enemy to the list 
    public void AddEnemy(BaseEnemy _enemyToAdd)
    {
        _enemyList.Add(_enemyToAdd);        //adds passed enemy into the list
    }

    //removes an enemy from the list
    public void RemoveMe(BaseEnemy _enemyToRemove)
    {
        _enemyList.Remove(_enemyToRemove);  //removes the enemy from the list
    }

    //removes a color pillar from the refernced list
    public void RemoveBlock(ColoredBlock _colorBlockToRemove)
    {
        _coloredBlocks.Remove(_colorBlockToRemove);     //removes a pillar from the list
    }

    //called to restart the room
    private void RestartRoom()
    {
        _myCollider.enabled = true;                             //turns collider on

        switch (_roomMechanic)
        {
            case Mechanic.NONE:                     
                break;
            case Mechanic.SWARM:                                //if swarm room

                EndAll();                                       //ends the room
                break;

            case Mechanic.CHASE:                                //if chase room

                if (_doors.Count > 0)                           //if there are doors
                {
                    for (int i = 0; i < _doors.Count; i++)
                    {
                        _doors[i].RoomDone();                   //raise all of the doors
                    }
                }
                for (int i = 0; i < _mazeCheckpoints.Count; i++)
                {
                    _mazeCheckpoints[i].MyReset();              //reset all of the checkpoints
                }   
                EndAll();                                       //ends the room
                break;

            case Mechanic.COLOR:                                //if color room

                EndAll();                                       //ends the room
                for (int i = 0; i < _coloredBlocks.Count; i++)
                {
                    _coloredBlocks[i].gameObject.SetActive(true);   //turn on all of the color pillars
                }

                for (int i = _offsetSpawnPointsHolder.Count-1; i >= 0; i--)
                {
                    _spawnPoints.Add(_offsetSpawnPointsHolder[i]);                  //add all points on the offset list to the main pillar list
                    _offsetSpawnPointsHolder.Remove(_offsetSpawnPointsHolder[i]);   //remove all points from the offset list
                }
                break;

            case Mechanic.TRAP:                                 //if trap room

                EndAll();                                       //ends the room
                for (int i = 0; i < _LeversToEndRoom.Count; i++)
                {
                    //Debug.Log("lever reseting");
                    _LeversToEndRoom[i].ResetLever();           //reset all trap levers
                }

                for (int i = 0; i < _trapsInRoom.Count; i++)
                {
                    _trapsInRoom[i].ResetTrap();                //reset all traps
                }
                break;

            case Mechanic.BOSS:

                _BigBad = _enemyPreFab.GetComponent<BossEnemy>();       //grabs boss in room                
                EndAll();                                                   //ends the room                                
                _BigBad.MyReset();                                      //resets the boss
                break;

            case Mechanic.HEAL:

                _healObject.SetActive(true);                            //turns on the heal aura
                _healObject.GetComponent<HealingGrace>().HealReset();   //resets the heal aura
                break;

            default:
                break;
        }
    }

    //turn the room off
    public void DisableRoom()
    {
        _myCollider.enabled = false;        //turn the collider off
        EndAll();                           //ends the room
    }

    public BossEnemy GetBigBoy { get { return _BigBad; } }
    public Mechanic GetMechanic { get { return _roomMechanic; } }
    public List<DoorMovement> GetDoors { get { return _doors; } }
    public List<BaseEnemy> GetCurrEnemyList { get { return _enemyList; } }
    public List<BaseTrap> GetCurrTrapList { get { return _trapsInRoom; } }
    public float GetChangeDistance { get { return _ghlostChangeDistance; } }
}