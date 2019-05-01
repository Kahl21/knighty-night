using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutManager : MonoBehaviour
{

    [Header("Breakout Blocks Variables")]
    [SerializeField]
    GameObject _blockPF;
    [SerializeField]
    List<Color> _rowsOfColors;
    [SerializeField]
    int _howManyPerRow;
    [SerializeField]
    float _horizontalSpawnOffset;
    [SerializeField]
    float _verticalSpawnOffset;
    [SerializeField]
    float _damagerToPlayer;
    [SerializeField]
    int _rowsToSkipForDeathPillars;
    List<BreakoutBlock> _blocksInScene = new List<BreakoutBlock>();
    List<BreakoutBlock> _deathBlocksInScene = new List<BreakoutBlock>();
    Vector3 _spawnPos;
    DungeonMechanic _myRoom;
    GameObject _puck;

    [Header("Camera Variables")]
    [SerializeField]
    CameraFollow _sceneCamera;
    Vector3 _initCamPosition;
    Vector3 _initCamEuler;
    [SerializeField]
    GameObject _minigameCamPos;
    Vector3 _minigameCamEuler;
    [SerializeField]
    float _animatedCamMoveDuration;

    bool started = false;
    bool ended = false;

    public void Init(DungeonMechanic dungeonMechanic)
    {
        _spawnPos = transform.position;
        _myRoom = dungeonMechanic;
        for (int row = 0; row < _rowsOfColors.Count; row++)
        {
            _spawnPos.z = transform.position.z - (_verticalSpawnOffset * row);
            SpawnRowOfBlocks(_rowsOfColors[row]);
        }
        SpawnDeathPillars();

        _initCamPosition = _sceneCamera.transform.position;
        _initCamEuler = _sceneCamera.transform.eulerAngles;

        _minigameCamEuler = _minigameCamPos.transform.eulerAngles;

        _sceneCamera.AmFollowingPlayer = false;
        _sceneCamera.GetComponent<CameraFollow>().BossIntroActive(_initCamPosition, _minigameCamPos.transform.position, _initCamEuler, _minigameCamEuler, _animatedCamMoveDuration);


        started = true;
    }

    private void Update()
    {
        if (started)
        {
            if (_sceneCamera.MoveCamera())
            {
                started = false;
            }
        }

        if (ended)
        {
            if (_sceneCamera.MoveCamera())
            {
                _sceneCamera.AmFollowingPlayer = true;
                ended = false;
            }
        }
    }

    private void SpawnRowOfBlocks(Color spawnColor)
    {
        for (int blockNum = 0; blockNum < _howManyPerRow; blockNum++)
        {
            _spawnPos.x = transform.position.x + (_horizontalSpawnOffset * blockNum);
            GameObject newBlock = Instantiate<GameObject>(_blockPF, _spawnPos, transform.rotation, null);
            _blocksInScene.Add(newBlock.GetComponent<BreakoutBlock>());
            newBlock.GetComponent<BreakoutBlock>().Init(this, spawnColor, _damagerToPlayer);
            newBlock.GetComponent<BreakoutBlock>().SetSpawner = _myRoom;
        }
    }

    private void SpawnDeathPillars()
    {
        for (int blockNum = 0; blockNum < _howManyPerRow; blockNum++)
        {
            _spawnPos.z = transform.position.z - (_verticalSpawnOffset * _rowsToSkipForDeathPillars);
            _spawnPos.x = transform.position.x + (_horizontalSpawnOffset * blockNum);
            GameObject newBlock = Instantiate<GameObject>(_blockPF, _spawnPos, transform.rotation, null);
            //_blocksInScene.Add(newBlock.GetComponent<BreakoutBlock>());
            newBlock.GetComponent<BreakoutBlock>().Init(this, Color.black, _damagerToPlayer);
            newBlock.GetComponent<BreakoutBlock>().SetSpawner = _myRoom;
            newBlock.GetComponent<BreakoutBlock>().SetDeathPillar = true;
            _deathBlocksInScene.Add(newBlock.GetComponent<BreakoutBlock>());
        }
    }

    public void EndRoom()
    {
        for (int index = 0; index < _deathBlocksInScene.Count; index++)
        {
            GameObject pillarObj;
            pillarObj = _deathBlocksInScene[index].gameObject;
            Destroy(pillarObj);
        }
        _sceneCamera.BossIntroActive(_sceneCamera.transform.position, PlayerController.Instance.transform.position + _sceneCamera.GetOffset, _sceneCamera.transform.eulerAngles, _initCamEuler, _animatedCamMoveDuration);
        ended = true;
        _deathBlocksInScene.Clear();
        Destroy(_puck);
    }

    public void ResetBreakout()
    {
        for (int index = 0; index < _blocksInScene.Count; index++)
        {
            GameObject pillarObj;
            pillarObj = _blocksInScene[index].gameObject;
            Destroy(pillarObj);
        }
        _blocksInScene.Clear();
        _sceneCamera.AmFollowingPlayer = true;
        _sceneCamera.transform.eulerAngles = _initCamEuler;

        for (int index = 0; index < _deathBlocksInScene.Count; index++)
        {
            GameObject pillarObj;
            pillarObj = _deathBlocksInScene[index].gameObject;
            Destroy(pillarObj);
        }
        _deathBlocksInScene.Clear();
        Destroy(_puck);
    }

    //private void 

    public List<BreakoutBlock> GetBlocksInScene { get { return _blocksInScene; } }
    public GameObject SetPuck { set { _puck = value; } }
}
