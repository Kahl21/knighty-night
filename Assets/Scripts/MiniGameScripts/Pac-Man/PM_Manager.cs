using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PM_Manager : MonoBehaviour
{
    private enum PHASES
    {
        NONE,
        MOVECAMERA,
        START,
        INITPHASE,
        COLOREDPHASE,
        KILLPHASE,
        END
    }

    [Header("Teleporters in the scene")]
    [SerializeField]
    [Tooltip("Make sure linked teleporters are next to each other \nand children of this gameobject")]
    List<GameObject> _teleporters;
    [SerializeField]
    GameObject _targetPointContainer;
    [SerializeField]
    List<GameObject> _targetPoints;
    [SerializeField]
    List<GameObject> _spawnPoints;
    [SerializeField]
    List<GameObject> _ghostsInScene;

    [Header("Spawner Variables")]
    [SerializeField]
    GameObject _ghostPrefab;
    [SerializeField]
    [Tooltip("This prefab needs to be disabled initially")]
    GameObject _coinLayoutPrefab;
    GameObject _currentCoins;
    [SerializeField]
    float _ghostSpawnDelay;
    [SerializeField]
    int _maxInitialGhostsInScene;

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

    [Header("Score Variables")]
    [SerializeField]
    GameObject _scorePrefab;
    GameObject _scoreCanvasInstance;
    [SerializeField]
    int _coinValue;
    [SerializeField]
    int _currentScore = 0;
    //[SerializeField]
    Text _scoreText;

    [Header("Phase 1 Variables")]
    [SerializeField]
    int _initialScoreNeeded = 0;

    [Header("Phase 2 Variables")]
    [SerializeField]
    List<GameObject> _coloredPillars;
    [SerializeField]
    List<Transform> _colorSpawnPoints;
    [SerializeField]
    GameObject _coloredGhlostPrefab;
    [SerializeField]
    float _travelRadius;

    [Header("Phase 3 Variables")]
    [SerializeField]
    float _finalGhostSpawnDelay;
    [SerializeField]
    int _finalMaxGhostsInScene;
    [SerializeField]
    Transform[] _swarmSpawnPoints;
    [SerializeField]
    GameObject _swordPowerUp;
    [SerializeField]
    Color _powerupSwordColor;
    [SerializeField]
    Color _initialSwordMat;
    float[] _spawnTimers;

    float _startTimer;
    float _currTime;
    int random;
    int spawnCounter = 0;
    PlayerController _playerRef;
    bool _playerHasSpecialSword = false;
    bool _ended = false;
    DungeonMechanic _mySpawner;
    PHASES _pmPhase = PHASES.NONE;

    // Use this for initialization
    void Start()
    {
        for (int index = 0; index < _teleporters.Count; index++)
        {
            _teleporters[index].SetActive(false);
        }

        for (int index = 0; index < _coloredPillars.Count; index++)
        {
            _coloredPillars[index].SetActive(false);
        }

        _scoreCanvasInstance = Instantiate(_scorePrefab);
        _scoreText = _scoreCanvasInstance.transform.GetChild(0).GetComponent<Text>();

        _scoreText.text = "Score: " + _currentScore + "/" + _initialScoreNeeded;
        _startTimer = Time.time;
        _ghostsInScene = new List<GameObject>();
        _playerRef = PlayerController.Instance;
        _spawnTimers = new float[_swarmSpawnPoints.Length];
        _initialSwordMat = _playerRef.gameObject.transform.GetChild(0).transform.GetChild(5).GetComponent<SkinnedMeshRenderer>().material.color;

        _targetPoints = new List<GameObject>();

        _currentCoins = Instantiate(_coinLayoutPrefab);
        _currentCoins.SetActive(true);

        foreach (Transform item in _targetPointContainer.transform.GetComponentsInChildren<Transform>())
        {
            _targetPoints.Add(item.gameObject);
        }
        _targetPoints.RemoveAt(0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_pmPhase)
        {
            case PHASES.NONE:
                break;

            case PHASES.MOVECAMERA:
                MoveCamera();
                break;

            case PHASES.START:
                Init();
                break;

            case PHASES.INITPHASE:
                spawnGhlost();
                break;

            case PHASES.COLOREDPHASE:
                spawnGhlost();
                CheckForPillarsCompleted();
                break;

            case PHASES.KILLPHASE:
                WavePhase();
                break;

            case PHASES.END:

                break;
        }

    }

    public void Init()
    {
        for (int index = 0; index < _teleporters.Count; index++)
        {
            _teleporters[index].SetActive(true);
        }

        _initCamPosition = _sceneCamera.transform.position;
        _initCamEuler = _sceneCamera.transform.eulerAngles;

        _minigameCamEuler = _minigameCamPos.transform.eulerAngles;

        _sceneCamera.AmFollowingPlayer = false;

        _sceneCamera.GetComponent<CameraFollow>().BossIntroActive(_initCamPosition, _minigameCamPos.transform.position, _initCamEuler, _minigameCamEuler, _animatedCamMoveDuration);

        PlayerController.Instance.AmInCutscene = true;

        _pmPhase = PHASES.MOVECAMERA;
    }

    private void MoveCamera()
    {
        if (_sceneCamera.MoveCamera() && _ended == false)
        {
            PlayerController.Instance.AmInCutscene = false;
            _pmPhase = PHASES.INITPHASE;
        }
        else if (_sceneCamera.MoveCamera() && _ended == true)
        {
            _sceneCamera.AmFollowingPlayer = true;
            PlayerController.Instance.AmInCutscene = false;
            _pmPhase = PHASES.NONE;
        }
    }

    //Needs to be setup for different spawn points
    private void spawnGhlost()
    {
        _currTime = Time.time - _startTimer;
        if (_currTime >= (_ghostSpawnDelay * spawnCounter) && _ghostsInScene.Count < _maxInitialGhostsInScene)
        {
            random = Random.Range(0, _spawnPoints.Count);

            GameObject ghostInstance;
            ghostInstance = Instantiate(_ghostPrefab);
            ghostInstance.transform.parent = null;
            ghostInstance.transform.position = _spawnPoints[random].transform.position;
            ghostInstance.GetComponent<PM_BasicGhlost>().BasicInit(GetComponent<PM_Manager>());
            _ghostsInScene.Add(ghostInstance);
            _startTimer = Time.time;
            spawnCounter++;
        }
    }

    private void CheckForPillarsCompleted()
    {
        
        int completedPillars = 0;
        for (int index = 0; index < _coloredPillars.Count; index++)
        {
            if (_coloredPillars[index].activeSelf == false)
            {
                completedPillars++;
            }
        }

        if (completedPillars == _coloredPillars.Count)
        {
            InitKillPhase();
        }
        
    }

    public void teleportObject(GameObject teleportingObj, GameObject enteredTeleporter)
    {
        for (int index = 0; index < _teleporters.Count; index++)
        {
            if (_teleporters[index] == enteredTeleporter)
            {

                if (index % 2 == 0)
                {
                    teleportingObj.transform.position = new Vector3(_teleporters[index + 1].transform.position.x, 0, _teleporters[index + 1].transform.position.z);
                    return;
                }
                else
                {
                    teleportingObj.transform.position = new Vector3(_teleporters[index - 1].transform.position.x, 0, _teleporters[index - 1].transform.position.z);
                    return;
                }
            }
        }
    }

    private void InitKillPhase()
    {
        //Initialize new set of coins
        Destroy(_currentCoins);
        _currentCoins = Instantiate(_coinLayoutPrefab);
        _currentCoins.SetActive(true);

        _swordPowerUp.SetActive(true);

        for (int index = 0; index < _spawnTimers.Length; index++)
        {
            _spawnTimers[0] = Time.time;
        }

        _pmPhase = PHASES.KILLPHASE;
    }

    private void WavePhase()
    {
        if (_playerHasSpecialSword == false)
        {
            for (int index = 0; index < _swarmSpawnPoints.Length; index++)
            {
                Debug.Log("Spawning from point " + index);
                if (_ghostsInScene.Count < _finalMaxGhostsInScene)
                {
                    _currTime = Time.time - _spawnTimers[index];
                    if (_currTime >= _finalGhostSpawnDelay && _ghostsInScene.Count < _finalMaxGhostsInScene)
                    {

                        GameObject ghostInstance;
                        ghostInstance = Instantiate(_ghostPrefab);
                        ghostInstance.transform.parent = null;
                        ghostInstance.transform.position = _swarmSpawnPoints[index].transform.position;
                        ghostInstance.GetComponent<PM_BasicGhlost>().BasicInit(GetComponent<PM_Manager>());
                        _ghostsInScene.Add(ghostInstance);
                        _spawnTimers[index] = Time.time;
                    }
                }
            }
        }
        else
        {
            if (_playerRef.gameObject.transform.GetChild(0).transform.GetChild(5).GetComponent<SkinnedMeshRenderer>().material.color != _powerupSwordColor)
            {
                _playerRef.gameObject.transform.GetChild(0).transform.GetChild(5).GetComponent<SkinnedMeshRenderer>().material.color = _powerupSwordColor;
            }
            if (_ghostsInScene.Count == 0)
            {
                _playerRef.gameObject.transform.GetChild(0).transform.GetChild(5).GetComponent<SkinnedMeshRenderer>().material.color = _initialSwordMat;
                Debug.Log("Level Complete");
                _ended = true;
                _mySpawner.CheckForEnd();
                _sceneCamera.BossIntroActive(_sceneCamera.transform.position, PlayerController.Instance.transform.position + _sceneCamera.GetOffset, _sceneCamera.transform.eulerAngles, _initCamEuler, _animatedCamMoveDuration);
                Destroy(_scoreCanvasInstance);
            }
        }
        
    }

    private void EnableColoredPillars()
    {
        for (int index = 0; index < _coloredPillars.Count; index++)
        {
            _coloredPillars[index].SetActive(true);
            GameObject coloredObj = Instantiate(_coloredGhlostPrefab);
            _ghostsInScene.Add(coloredObj);
            coloredObj.transform.position = _colorSpawnPoints[index].position;
            coloredObj.GetComponent<PM_ColoredGhlost>().InitColor(_coloredPillars[index].GetComponent<ColoredBlock>().GetColor, GetComponent<PM_Manager>(), _travelRadius, _coloredPillars[index]);
        }
        _pmPhase = PHASES.COLOREDPHASE;
    }

    public void DisablePillar(GameObject pillar)
    {
        pillar.SetActive(false);
    }

    public void AddToScore(GameObject coin)
    {


        if (coin.GetComponent<Collectable>() && coin.GetComponent<Collectable>().IsSword != true)
        {
            _currentScore += _coinValue;
            _scoreText.text = "Score: " + _currentScore + "/" + _initialScoreNeeded;
        }
        else if(coin.GetComponent<Collectable>() && coin.GetComponent<Collectable>().IsSword == true)
        {
            _playerHasSpecialSword = true;
        }
        else
        {
            if (coin.GetComponent<PM_BasicGhlost>())
            {
                _ghostsInScene.Remove(coin);
            }
            _currentScore += _coinValue;
            _scoreText.text = "Score: " + _currentScore + "/" + _initialScoreNeeded;
        }

        if (_pmPhase == PHASES.INITPHASE && _currentScore>=_initialScoreNeeded)
        {
            Debug.Log("Enough Coins reached");
            EnableColoredPillars();
            _pmPhase = PHASES.COLOREDPHASE;
        }
    }

    public void EndMinigame()
    {
        _pmPhase = PHASES.MOVECAMERA;
        PlayerController.Instance.AmInCutscene = true;
    }

    public void MyReset()
    {
        for (int index = 0; index < _ghostsInScene.Count; index++)
        {
            Destroy(_ghostsInScene[index]);
        }
        _ghostsInScene.Clear();

        _pmPhase = PHASES.NONE;
        Destroy(_currentCoins);
        _currentCoins = Instantiate(_coinLayoutPrefab);
        _currentCoins.SetActive(true);
        _swordPowerUp.SetActive(false);
        _playerHasSpecialSword = false;

        _currentScore = 0;
        _scoreText.text = "Score: " + _currentScore + "/" + _initialScoreNeeded;
        Destroy(_scoreCanvasInstance);

        _sceneCamera.AmFollowingPlayer = true;

        Debug.Log("RESET PACMAN");
        Start();
    }

    public List<GameObject> GetTargetPoints { get { return _targetPoints; } }
    public bool PlayerHasSpecialSword { get { return _playerHasSpecialSword; } }
    public DungeonMechanic SetSpawner { set { _mySpawner = value; } }
}
