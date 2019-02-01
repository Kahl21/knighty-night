﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MenuOrient
{
    VERT,
    HORIZ
}

public enum WhichUIMenu
{
    MAINMENU,
    OPTIONS,
    PLAYER,
    PAUSE,
    WIN
}


public class Menuing : MonoBehaviour {

    private static Menuing _instance;
    public static Menuing Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                if (FindObjectOfType<Menuing>())
                {
                    _instance = FindObjectOfType<Menuing>();
                    return _instance;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    [Header("Base Menu Variables")]
    [SerializeField]
    List<GameObject> _menus;
    GameObject _BossBar;
    [SerializeField]
    List<Selectable> currSelectableButtons;
    int currSelected = 0;
    bool _canMenu = true;
    bool _paused = false;

    [SerializeField]
    float _inputDelayDuration;
    float _currDelayTime;
    bool _menuDelay;
    float fakeTimeDotTime = 1;

    [Header("Credits Variables")]
    [SerializeField]
    GameObject _credits;
    Vector3 _creditsStartPos, c0, c1;
    [SerializeField]
    float _creditsVertDistance;
    [SerializeField]
    float _creditsDuration;
    float _currTime;
    float _startTime;
    bool _creditsRolling;

    PlayerController _playerRef;
    GameManager _managerRef;

	// Use this for initialization
	void Awake ()
    {
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Debug.Log("player copy Destroyed");

            Destroy(gameObject);
        }

        _menus = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            _menus.Add(transform.GetChild(i).gameObject);
        }
        _BossBar = _menus[2].transform.GetChild(3).gameObject;
        _BossBar.SetActive(false);

        _creditsStartPos = _credits.transform.localPosition;
        _credits.SetActive(false);
        _creditsRolling = false;

        _managerRef = GameManager.Instance;
        _playerRef = PlayerController.Instance;
        _playerRef.SetMenus = _menus;

        SetMenu(WhichUIMenu.WIN);

        _playerRef.SetWinImage = _menus[(int)WhichUIMenu.WIN].transform.GetChild(1).gameObject;
        _playerRef.SetLoseImage = _menus[(int)WhichUIMenu.WIN].transform.GetChild(2).gameObject;

        SetMenu(WhichUIMenu.MAINMENU);
    }

    private void Update()
    {
        if(_menuDelay)
        {
            MenuDelay();
        }

        if(_creditsRolling)
        {
            RollCredits();
        }
    }

    public void SetMenu(WhichUIMenu _whichMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _menus[i].SetActive(false);
        }
        _menus[(int)_whichMenu].SetActive(true);

        if (_whichMenu != WhichUIMenu.PLAYER)
        {
            SetButtons((int)_whichMenu);
            currSelectableButtons[currSelected].Select();
        }
    }

    private void SetButtons(int _menuNum)
    {
        if(_menuNum != 4)
        {
            _playerRef.SetOrientation = MenuOrient.VERT;
        }
        else
        {
            _playerRef.SetOrientation = MenuOrient.HORIZ;
        }
        currSelectableButtons = new List<Selectable>();
        for (int i = 0; i < _menus[_menuNum].transform.childCount; i++)
        {
            if (_menus[_menuNum].transform.GetChild(i).GetComponent<Selectable>())
            {
                currSelectableButtons.Add(_menus[_menuNum].transform.GetChild(i).GetComponent<Selectable>());
            }
        }
        currSelected = 0;
    }

    public void Pause()
    {
        if(_paused)
        {
            SetMenu(WhichUIMenu.PLAYER);
            Time.timeScale = 1;
            _playerRef.InMenu = false;
            _paused = false;
        }
        else
        {
            SetMenu(WhichUIMenu.PAUSE);
            Time.timeScale = 0;
            _playerRef.InMenu = true;
            _paused = true;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        _paused = false;

        _playerRef.InMenu = false;
        _playerRef.ResetPlayer();
        SetMenu(WhichUIMenu.PLAYER);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);

        _managerRef.SetGameReset = _playerRef.ResetPlayer;
        _playerRef.InMenu = false;
        Time.timeScale = 1;
        _paused = false;
        SetMenu(WhichUIMenu.PLAYER);
    }

    public void RetryLevel()
    {
        SetMenu(WhichUIMenu.PLAYER);
        _playerRef.InMenu = false;
        Time.timeScale = 1;
        _paused = false;
        _managerRef.ResetGame();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
        SetMenu(WhichUIMenu.PLAYER);
        _playerRef.InMenu = false;
        Time.timeScale = 1;
        _paused = false;
        _managerRef.SetGameReset = _playerRef.ResetPlayer;
        _managerRef.ResetGame();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        _paused = false;


        _managerRef.SetGameReset = _playerRef.ResetPlayer;
        _playerRef.ResetPlayer();

        _playerRef.transform.position = _playerRef.GetMenuPos;
        _playerRef.transform.rotation = _playerRef.GetMenuRot;
        _playerRef.InMenu = true;
        _playerRef.GetPlayerAnimator.Play("Nothing", 0);

        _BossBar.SetActive(false);
        SetMenu(0);
    }

    public void ToOptions()
    {
        SetMenu(WhichUIMenu.OPTIONS);
    }

    public void MenuBack()
    {
        SetMenu(WhichUIMenu.MAINMENU);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void MenuUpOrDown(bool _positiveMovement)
    {
        //Debug.Log(_positiveMovement);
        if(_canMenu)
        {
            _canMenu = false;
            if (_positiveMovement)
            {
                currSelected++;
                if (currSelected > currSelectableButtons.Count - 1)
                {
                    currSelected = 0;
                }
            }
            else
            {
                currSelected--;
                if (currSelected < 0)
                {
                    currSelected = currSelectableButtons.Count - 1;
                }
            }
            
            currSelectableButtons[currSelected].Select();
            fakeTimeDotTime = 1;
            _menuDelay = true;
        }
    }

    public void SelectButton()
    {
        if(currSelectableButtons[currSelected].GetComponent<Toggle>())
        {
            currSelectableButtons[currSelected].GetComponent<Toggle>().isOn = !currSelectableButtons[currSelected].GetComponent<Toggle>().isOn;
        }
        else
        {
            currSelectableButtons[currSelected].GetComponent<Button>().onClick.Invoke();
        }
    }

    void MenuDelay()
    {
        _currDelayTime = fakeTimeDotTime / _inputDelayDuration;

        if(_currDelayTime >= 1)
        {
            _currDelayTime = 1;

            _menuDelay = false;
            _canMenu = true;
        }
        else
        {
            fakeTimeDotTime++;
        }
    }

    public void StartCredits()
    {
        _credits.SetActive(true);
        c0 = _credits.transform.localPosition;
        c1 = _credits.transform.localPosition + (Vector3.up * _creditsVertDistance);
        _startTime = Time.time;
        _creditsRolling = true;
    }

    void RollCredits()
    {
        _currTime = (Time.time - _startTime) / _creditsDuration;

        if(_currTime >= 1)
        {
            _currTime = 1;
        }
        else
        {
            Vector3 p01;

            p01 = (1 - _currTime) * c0 + _currTime * c1;

            _credits.transform.localPosition = p01;
        }
    }

    public void StopCredits()
    {
        _credits.transform.localPosition = _creditsStartPos;
        _credits.SetActive(false);
        _creditsRolling = false;
    }

     
    public PlayerController SetPlayer { set { _playerRef = value; } }
    public List<GameObject> GetMenus { get { return _menus; } }
    public GameObject GetBossBar { get { return _BossBar; } }

    public bool GameIsPaused { get { return _paused; } } 
    public bool AreCreditsRolling { get { return _creditsRolling; } }
}
