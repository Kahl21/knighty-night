using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PM_Manager : MonoBehaviour
{
    private enum PHASES
    {
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

    [Header("Score Variables")]
    [SerializeField]
    int _coinValue;
    [SerializeField]
    int _currentScore = 0;
    [SerializeField]
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

    float _startTimer;
    float _currTime;
    int random;
    PlayerController _playerRef;
    bool _playerHasSpecialSword = false;
    PHASES _pmPhase = PHASES.START;

    // Use this for initialization
    void Start()
    {
        _scoreText.text = "Score: " + _currentScore + "/" + _initialScoreNeeded;
        _startTimer = Time.time;
        _ghostsInScene = new List<GameObject>();
        _playerRef = PlayerController.Instance;

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
            case PHASES.START:
                Init();
                break;

            case PHASES.INITPHASE:
                spawnGhlost(_ghostSpawnDelay, _maxInitialGhostsInScene);
                break;

            case PHASES.COLOREDPHASE:
                spawnGhlost(_ghostSpawnDelay, _maxInitialGhostsInScene);
                CheckForPillarsCompleted();
                break;

            case PHASES.KILLPHASE:
                WavePhase();
                break;

            case PHASES.END:

                break;
        }

    }

    private void Init()
    {
        for (int index = 0; index < _coloredPillars.Count; index++)
        {
            _coloredPillars[index].SetActive(false);
        }
        _pmPhase = PHASES.INITPHASE;
    }

    //Needs to be setup for different spawn points
    private void spawnGhlost(float spawnDelay, float maxInScene)
    {
        _currTime = Time.time - _startTimer;
        if (_currTime >= spawnDelay && _ghostsInScene.Count < maxInScene)
        {
            random = Random.Range(0, _spawnPoints.Count);
            GameObject ghostInstance;
            ghostInstance = Instantiate(_ghostPrefab);
            ghostInstance.transform.parent = null;
            ghostInstance.transform.position = _spawnPoints[random].transform.position;
            ghostInstance.GetComponent<PM_BasicGhlost>().BasicInit(GetComponent<PM_Manager>());
            _ghostsInScene.Add(ghostInstance);
            _startTimer = Time.time;
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

        _pmPhase = PHASES.KILLPHASE;
    }

    private void WavePhase()
    {
        if (_playerHasSpecialSword == false)
        {
            for (int index = 0; index < _swarmSpawnPoints.Length; index++)
            {
                if (_ghostsInScene.Count < _finalMaxGhostsInScene)
                {
                    spawnGhlost(_finalGhostSpawnDelay, _finalMaxGhostsInScene);
                }
            }
        }
        else
        {
            if (_ghostsInScene.Count == 0)
            {
                Debug.Log("Level Complete");
            }
        }
        
    }

    private void EnableColoredPillars()
    {
        for (int index = 0; index < _coloredPillars.Count; index++)
        {
            _coloredPillars[index].SetActive(true);
            GameObject coloredObj = Instantiate(_coloredGhlostPrefab);
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
        _currentScore += _coinValue;
        _scoreText.text = "Score: " + _currentScore + "/" + _initialScoreNeeded;

        if (_pmPhase == PHASES.INITPHASE && _currentScore>=_initialScoreNeeded)
        {
            Debug.Log("Enough Coins reached");
            EnableColoredPillars();
            _pmPhase = PHASES.COLOREDPHASE;
        }
    }

    public List<GameObject> GetTargetPoints { get { return _targetPoints; } }
    public bool PlayerHasSpecialSword { get { return _playerHasSpecialSword; } }
}
