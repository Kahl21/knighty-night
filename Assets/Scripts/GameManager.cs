using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RestartDelegate();
public class GameManager : MonoBehaviour {

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

    public PlayerController GetPlayer { get { return _playerRef; } }
    public bool HardModeOn { get { return _hardMode; } set { _hardMode = value; } }
    public bool HasSubtitles { get { return _subtitles; } set { _subtitles = value; } }

}
