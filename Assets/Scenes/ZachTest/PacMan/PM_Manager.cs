using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_Manager : MonoBehaviour
{
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

    float _startTimer;
    float _currTime;
    int random;

    // Use this for initialization
    void Start()
    {
        _startTimer = Time.time;
        _ghostsInScene = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnGhlost();
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

    public List<GameObject> GetTargetPoints { get { return _targetPoints; } }
}
