using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour {

    PlayerController _player;
    Vector3 _playerStartPos;

    int currscene;
    [Tooltip("The highest number on a scene in the build settings")]
    [SerializeField]
    int _maxSceneNum;

    private void Awake()
    {
        currscene = SceneManager.GetActiveScene().buildIndex;
        _player = PlayerController.Instance;
        _playerStartPos = transform.GetChild(0).transform.position;
        _playerStartPos.y = 0;
        _player.SetStartPos = _playerStartPos;
        _player.transform.position = _playerStartPos;
    }

    public bool IsLastLevel()
    {
        if(currscene == _maxSceneNum)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
