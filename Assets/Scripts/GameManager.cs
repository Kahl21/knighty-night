using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RestartDelegate();
public class GameManager : MonoBehaviour {

    public static bool[] _themesUnlocked;
    public static int _lastLevelIndex;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(_instance != null)
            {
                return _instance;
            }
            else
            {
                if(FindObjectOfType<GameManager>())
                {
                    _instance = FindObjectOfType<GameManager>();
                    return _instance;
                }
                else
                {
                    Debug.Log("there's nothing here");
                    return null;
                }
            }
        }
    }

    
    private RestartDelegate Reset;
    public RestartDelegate SetGameReset { get { return Reset; } set { Reset = value; } }

    PlayerController _playerRef;
    Menuing _menuRef;

    [SerializeField]
    bool _subtitles = true;
    [SerializeField]
    bool _hardMode = false;

    private void Awake()
    {
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _playerRef = PlayerController.Instance;
        _menuRef = Menuing.Instance;
    }

    public void ResetGame()
    {
        if(Reset != null)
        {
            Reset.Invoke();
        }
    }

    public bool[] CreateSave()
    {
        _themesUnlocked = new bool[4];

        for (int index = 0; index < _themesUnlocked.Length; index++)
        {
            _themesUnlocked[index] = false;
        }
        
        return _themesUnlocked;
    }

    public void SaveCompletedRoom(int currentScene)
    {
        _lastLevelIndex = currentScene;
    }

    public void CheckForCompletedWorld(int currentScene)
    {
        if (currentScene == 4)
        {
            _themesUnlocked[0] = true;
        }
        else if (currentScene == 7)
        {
            _themesUnlocked[1] = true;
        }
        else if (currentScene == 10)
        {
            _themesUnlocked[2] = true;
        }
        else if (currentScene == 13)
        {
            _themesUnlocked[3] = true;
        }
    }
    

    public PlayerController GetPlayer { get { return _playerRef; } }
    public bool HardModeOn { get { return _hardMode; } set { _hardMode = value; } }
    public bool HasSubtitles { get { return _subtitles; } set { _subtitles = value; } }

}
