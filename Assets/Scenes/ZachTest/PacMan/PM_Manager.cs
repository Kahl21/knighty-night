using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [Tooltip("Make sure linked teleporters are next to each other\nand children of this gameobject")]
    List<GameObject> _teleporters;
    [SerializeField]
    List<GameObject> _targetPoints;
    [SerializeField]
    List<GameObject> _spawnPoints;
    List<GameObject> _ghostsInScene;

    [Header("Spawner Variables")]
    [SerializeField]
    GameObject _ghostPrefab;
    [SerializeField]
    float ghostSpawnDelay;
    [SerializeField]
    int maxGhostsInScene;

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
    List<Vector3> _colorSpawnPoints;
    [SerializeField]
    GameObject _coloredGhlostPrefab;

    float _startTimer;
    float _currTime;
    int random;
    PlayerController _playerRef;
    PHASES _pmPhase = PHASES.INITPHASE;

    // Use this for initialization
    void Start()
    {
        _scoreText.text = "Score: " + _currentScore + "/" + _initialScoreNeeded;
        _startTimer = Time.time;
        _ghostsInScene = new List<GameObject>();
        _playerRef = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_pmPhase)
        {
            case PHASES.START:

                break;

            case PHASES.INITPHASE:
                spawnGhlost();
                break;

            case PHASES.COLOREDPHASE:
                CheckForPillarsCompleted();
                break;

            case PHASES.KILLPHASE:

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

    void spawnGhlost()
    {
        _currTime = Time.time - _startTimer;
        if (_currTime >= ghostSpawnDelay && _ghostsInScene.Count < maxGhostsInScene)
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
            if (_coloredPillars[index])
            {
                completedPillars++;
            }
        }

        if (completedPillars == _coloredPillars.Count)
        {
            _pmPhase = PHASES.KILLPHASE;
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

    //Finish this. Need to initialize the colored ghlosts color and start their moving process
    private void EnableColoredPillars()
    {
        for (int index = 0; index < _coloredPillars.Count; index++)
        {
            _coloredPillars[index].SetActive(true);
            GameObject coloredObj = Instantiate(_coloredGhlostPrefab);
            coloredObj.transform.position = _colorSpawnPoints[index];
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
        Destroy(coin);

        if (_pmPhase == PHASES.INITPHASE && _currentScore>=_initialScoreNeeded)
        {
            EnableColoredPillars();
        }
    }

    public List<GameObject> GetTargetPoints { get { return _targetPoints; } }
}
