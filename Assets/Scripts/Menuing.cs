using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MenuOrient
{
    VERT,
    HORIZ
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

    //loadscreen variables
    public Image _loadScreen;
    public Image _fadeScreen;
    private bool isLoading = false;
    private Color blackColor = new Color(0, 0, 0, 1);
    private Color transparentColor = new Color(0, 0, 0, 0);
    private Color fullColor = new Color(1, 1, 1, 1);
    private float fadeTime = 1f;
    private float fadeUpdateTime = .01f;

    //Video Settings
    bool isFullscreen;
    public Text currenResolution;
    public Text currentWindow;


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

        SetMenu(4);

        _playerRef.SetWinImage = _menus[4].transform.GetChild(1).gameObject;
        _playerRef.SetLoseImage = _menus[4].transform.GetChild(2).gameObject;

        if (Screen.fullScreen == true)
        {
            currentWindow.text = "Fullscreen";
        }

        if (Screen.fullScreen == false)
        {
            currentWindow.text = "Windowed";
        }

        currenResolution.text = Screen.width + " x " + Screen.height;

        SetMenu(0);

    }

    private void Update()
    {
        Fading();
        if(_menuDelay)
        {
            MenuDelay();
        }

        if(_creditsRolling)
        {
            RollCredits();
        }
    }

    public void SetMenu(int _whichMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _menus[i].SetActive(false);
        }
        _menus[_whichMenu].SetActive(true);
        _menus[5].SetActive(true);

        if (_whichMenu != 2)
        {
            SetButtons(_whichMenu);
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
            SetMenu(2);
            Time.timeScale = 1;
            _playerRef.InMenu = false;
            _paused = false;
        }
        else
        {
            SetMenu(3);
            Time.timeScale = 0;
            _playerRef.InMenu = true;
            _paused = true;
        }
    }

    public void StartGame()
    {
        //SceneManager.LoadScene(1);
        StartCoroutine(LoadNewScene());
        Time.timeScale = 1;
        _paused = false;

        _playerRef.InMenu = false;
        _playerRef.ResetPlayer();
        SetMenu(2);
    }

    public void NextLevel()
    {

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        if (isLoading == false)
        {
            _menus[5].SetActive(true);
            StartCoroutine(LoadNewScene());
            
        }

        _managerRef.SetGameReset = _playerRef.ResetPlayer;
        _playerRef.InMenu = false;
        Time.timeScale = 1;
        _paused = false;
        SetMenu(2);
    }

    public void RetryLevel()
    {
        SetMenu(2);
        _playerRef.InMenu = false;
        Time.timeScale = 1;
        _paused = false;
        _managerRef.ResetGame();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
        SetMenu(2);
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
        SetMenu(1);
    }

    public void ToVideo()
    {
        SetMenu(6);
    }

    public void ToAudio()
    {
        SetMenu(7);
    }

    public void MenuBack()
    {
        SetMenu(0);
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

    public void Fading()
    {
       if(!isLoading && _fadeScreen.color != transparentColor)
        {
            StartCoroutine(FadeIn());
        }


    }


    //video Settings
    private void ChangeResolution(int width, int height, bool fullscreen)
    {
        Screen.SetResolution(width, height, fullscreen);
        
    }

    public void ChangeWindow()
    {
        if(Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
            isFullscreen = false;
            currentWindow.text = "Windowed";
        }
        else
        {
            Screen.fullScreen = true;
            isFullscreen = true;
            currentWindow.text = "FullScreen";

        }
    }

    public void ChooseResolution()
    {
        SetMenu(8);

    }

    public void Resolution1920x1080 ()
    {
        ChangeResolution(1920, 1080, isFullscreen);
        currenResolution.text = "1920 x 1080";
        SetMenu(6);
    }

    public void Resolution1920x1200()
    {
        ChangeResolution(1920, 1200, isFullscreen);
        currenResolution.text = "1920 x 1200";
        SetMenu(6);
    }

    public void Resolution1600x900()
    {
        ChangeResolution(1600, 900, isFullscreen);
        currenResolution.text = "1600 x 600";
        SetMenu(6);
    }

    public void Resolution1440x900()
    {
        ChangeResolution(1440, 900, isFullscreen);
        currenResolution.text = "1440 x 600";
        SetMenu(6);
    }

    public void Resolution1366x768()
    {
        ChangeResolution(1366, 768, isFullscreen);
        currenResolution.text = "1366 x 768";
        SetMenu(6);
    }

    public void Resolution1280x1024()
    {
        ChangeResolution(1280, 1024, isFullscreen);
        currenResolution.text = "1280 x 1024";
        SetMenu(6);
    }

    public void Resolution1280x768()
    {
        ChangeResolution(1280, 768, isFullscreen);
        currenResolution.text = "1280 x 768";
        SetMenu(6);
    }

    public void Resolution1024x768()
    {
        ChangeResolution(1024, 768, isFullscreen);
        currenResolution.text = "1024 x 768";
        SetMenu(6);
    }

    public void Resolution800x600()
    {
        ChangeResolution(800, 600, isFullscreen);
        currenResolution.text = "800 x 600";
        SetMenu(6);
    }






    //audio Settings
    public void MuteAll()
    {
        AudioManager.instance.MasterVolume(0);
    }

    public void MuteSFX()
    {
        AudioManager.instance.SFXVolume(0);
    }

    public void MuteMusic()
    {
        AudioManager.instance.MusicVolume(0);

    }

    public void ChangeMasterVolume(int vol)
    {
        AudioManager.instance.MasterVolume(vol);
    }

    public void ChangeSFXVolume(int vol)
    {
        AudioManager.instance.SFXVolume(vol);
    }

    public void ChangeMusicVolume(int vol)
    {
        AudioManager.instance.MusicVolume(vol);

    }









    IEnumerator LoadNewScene()
    {
        _fadeScreen.color = transparentColor;
        float time = 0;
        Color wantedColor = _fadeScreen.color;
        isLoading = true;
        AsyncOperation asyncLoad  = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        asyncLoad.allowSceneActivation = false;

        //yield return new WaitForSeconds(3f);
        while(time < fadeTime)
        {
            yield return new WaitForSeconds(fadeUpdateTime);

            wantedColor = Color.Lerp(transparentColor, blackColor, time);
            _fadeScreen.color = wantedColor;
            time += fadeUpdateTime;
        }
        _loadScreen.color = fullColor;
        yield return new WaitForSeconds(2f);
        _loadScreen.color = transparentColor;
        asyncLoad.allowSceneActivation = true;
        isLoading = false;
    }

    IEnumerator FadeIn()
    {
        float time = 0;
        Color wantedColor = _fadeScreen.color;

        while (time < fadeTime)
        {
            yield return new WaitForSeconds(fadeUpdateTime);

            wantedColor = Color.Lerp(blackColor, transparentColor, time);
            _fadeScreen.color = wantedColor;
            time += fadeUpdateTime;
        }
    }



    public PlayerController SetPlayer { set { _playerRef = value; } }
    public List<GameObject> GetMenus { get { return _menus; } }
    public GameObject GetBossBar { get { return _BossBar; } }

    public bool GameIsPaused { get { return _paused; } } 
    public bool AreCreditsRolling { get { return _creditsRolling; } }
}
