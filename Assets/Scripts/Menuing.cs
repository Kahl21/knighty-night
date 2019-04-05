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

public enum WhichUIMenu
{
    MAINMENU,
    OPTIONS,
    PLAYER,
    PAUSE,
    WIN,
    LOAD,
    VIDEO,
    AUDIO,
    RESOLUTION,
    MASTER,
    MUSIC,
    SFX,
    AREYOUSURE
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


    //Audio Settings
    GameObject _Audio;
    AudioManager _audioManager;
    public Text currentMaster;
    public Text currentMusic;
    public Text currentSFX;
    public Text changedMaster;
    public Text changedMusic;
    public Text changedSFX;
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
        _BossBar = _menus[(int)WhichUIMenu.PLAYER].transform.GetChild(3).gameObject;
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

        if (Screen.fullScreen == true)
        {
            currentWindow.text = "Fullscreen";
        }

        if (Screen.fullScreen == false)
        {
            currentWindow.text = "Windowed";
        }

        _Audio = GameObject.Find("AudioManager");
        _audioManager = _Audio.GetComponent<AudioManager>();
        currenResolution.text = Screen.width + " x " + Screen.height;


        SetMenu((int)WhichUIMenu.MAINMENU);

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
        changedMaster.text = _audioManager.volMaster.ToString();
        changedMusic.text = _audioManager.volMusic.ToString();
        changedSFX.text = _audioManager.volSFX.ToString();
        currentMaster.text = changedMaster.text;
        currentMusic.text = changedMusic.text;
        currentSFX.text = changedSFX.text;
    }

    public void SetMenu(WhichUIMenu _whichMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _menus[i].SetActive(false);
        }
        _menus[(int)_whichMenu].SetActive(true);
        _menus[(int)WhichUIMenu.LOAD].SetActive(true);

        if (_whichMenu != WhichUIMenu.PLAYER)
        {
            SetButtons((int)_whichMenu);
            currSelectableButtons[currSelected].Select();
        }
    }

    private void SetButtons(int _menuNum)
    {
        if(_menuNum == (int)WhichUIMenu.WIN || _menuNum == (int)WhichUIMenu.MASTER || _menuNum == (int)WhichUIMenu.MUSIC || _menuNum == (int)WhichUIMenu.SFX || _menuNum == (int)WhichUIMenu.AREYOUSURE)
        {
            _playerRef.SetOrientation = MenuOrient.HORIZ;
        }
        else
        {
            _playerRef.SetOrientation = MenuOrient.VERT;
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
        AudioManager.instance.ButtonPressed();
        if (_paused)
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
        AudioManager.instance.ButtonPressed();
        StartCoroutine(LoadNewScene());
        Time.timeScale = 1;
        _paused = false;

        _playerRef.GetCurrCheckpoint = 0;
        _playerRef.DoesHaveCheckpoint = false;
        _playerRef.InMenu = false;
        _playerRef.ResetPlayer();
        SetMenu(WhichUIMenu.PLAYER);
    }

    public void NextLevel()
    {

        if (isLoading == false)
        {
            _menus[(int)WhichUIMenu.VIDEO].SetActive(true);
            StartCoroutine(LoadNewScene());
            
        }

        _managerRef.SetGameReset = _playerRef.ResetPlayer;
        _playerRef.GetCurrCheckpoint = 0;
        _playerRef.DoesHaveCheckpoint = false;
        _playerRef.InMenu = false;
        Time.timeScale = 1;
        _paused = false;
        SetMenu(WhichUIMenu.PLAYER);
    }

    public void RetryLevel()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.PLAYER);
        _playerRef.InMenu = false;
        Time.timeScale = 1;
        _paused = false;
        _managerRef.ResetGame();
    }

    public void BackToMainMenu()
    {
        AudioManager.instance.ButtonPressed();
        //SceneManager.LoadScene(0);
        StartCoroutine(LoadSpecificScene(0));
        Time.timeScale = 1;
        _paused = false;


        _managerRef.SetGameReset = _playerRef.ResetPlayer;
        _playerRef.ResetPlayer();

        _playerRef.transform.position = _playerRef.GetMenuPos;
        _playerRef.transform.rotation = _playerRef.GetMenuRot;
        _playerRef.InMenu = true;
        _playerRef.GetPlayerAnimator.Play("Nothing", 0);

        _BossBar.SetActive(false);
        SetMenu(WhichUIMenu.MAINMENU);
    }

    public void ToOptions()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.OPTIONS);
    }

    public void ToVideo()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void ToAudio()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.AUDIO);
    }

    public void ToMaster()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.MASTER);
    }

    public void ToMusic()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.MUSIC);
    }

    public void ToSFX()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.SFX);
    }

    public void ToAreYouSure()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.AREYOUSURE);
    }



    public void MenuBack()
    {
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.MAINMENU);
    }

    public void EndGame()
    {
        AudioManager.instance.ButtonPressed();
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
            StopCoroutine(FadeOut());
            StartCoroutine(FadeIn());
        }

       if(isLoading && _fadeScreen.color != blackColor)
        {
            StopCoroutine(FadeIn());
            StartCoroutine(FadeOut());
        }

    }


    //video Settings
    private void ChangeResolution(int width, int height, bool fullscreen)
    {
        AudioManager.instance.ButtonPressed();
        Screen.SetResolution(width, height, fullscreen);
        
    }

    public void ChangeWindow()
    {
        AudioManager.instance.ButtonPressed();
        if (Screen.fullScreen == true)
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
        AudioManager.instance.ButtonPressed();
        SetMenu(WhichUIMenu.RESOLUTION);

    }

    public void Resolution1920x1080 ()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1920, 1080, isFullscreen);
        currenResolution.text = "1920 x 1080";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution1920x1200()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1920, 1200, isFullscreen);
        currenResolution.text = "1920 x 1200";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution1600x900()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1600, 900, isFullscreen);
        currenResolution.text = "1600 x 600";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution1440x900()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1440, 900, isFullscreen);
        currenResolution.text = "1440 x 600";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution1366x768()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1366, 768, isFullscreen);
        currenResolution.text = "1366 x 768";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution1280x1024()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1280, 1024, isFullscreen);
        currenResolution.text = "1280 x 1024";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution1280x768()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1280, 768, isFullscreen);
        currenResolution.text = "1280 x 768";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution1024x768()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(1024, 768, isFullscreen);
        currenResolution.text = "1024 x 768";
        SetMenu(WhichUIMenu.VIDEO);
    }

    public void Resolution800x600()
    {
        AudioManager.instance.ButtonPressed();
        ChangeResolution(800, 600, isFullscreen);
        currenResolution.text = "800 x 600";
        SetMenu(WhichUIMenu.VIDEO);
    }






    //audio Settings
    public void MuteAll()
    {
        AudioManager.instance.ButtonPressed();
        _audioManager.MasterVolume(0);
    }

    public void MuteSFX()
    {
        AudioManager.instance.ButtonPressed();
        _audioManager.SFXVolume(0);
    }

    public void MuteMusic()
    {
        AudioManager.instance.ButtonPressed();
        _audioManager.MusicVolume(0);

    }

    private void ChangeMasterVolume(float vol)
    {
        AudioManager.instance.ButtonPressed();
        _audioManager.MasterVolume(vol);
    }

    private void ChangeSFXVolume(float vol)
    {
        AudioManager.instance.ButtonPressed();
        _audioManager.SFXVolume(vol);
    }

    private void ChangeMusicVolume(float vol)
    {
        AudioManager.instance.ButtonPressed();
        _audioManager.MusicVolume(vol);

    }

    public void MasterUp()
    {
        AudioManager.instance.ButtonPressed();
        float currentVol = _audioManager.volMaster;
        if (currentVol < 10)
            ChangeMasterVolume(currentVol+1);
        changedMaster.text = _audioManager.volMaster.ToString();
    }

    public void MasterDown()
    {
        AudioManager.instance.ButtonPressed();
        float currentVol = _audioManager.volMaster;
        if (currentVol > 0)
            ChangeMasterVolume(currentVol-1);
        changedMaster.text = _audioManager.volMaster.ToString();
    }

    public void MusicUp()
    {
        AudioManager.instance.ButtonPressed();
        float currentVol = _audioManager.volMusic;
        if (currentVol < 10)
            ChangeMusicVolume(currentVol+1);
        changedMusic.text = _audioManager.volMusic.ToString();
    }

    public void MusicDown()
    {
        AudioManager.instance.ButtonPressed();
        float currentVol = _audioManager.volMusic;
        if (currentVol > 0)
            ChangeMusicVolume(currentVol-1);
        changedMusic.text = _audioManager.volMusic.ToString();
    }

    public void SFXUp()
    {
        AudioManager.instance.ButtonPressed();
        float currentVol = _audioManager.volSFX;
        if (currentVol < 10)
            ChangeSFXVolume(currentVol+1);
        changedSFX.text = _audioManager.volSFX.ToString();
    }

    public void SFXDown()
    {
        AudioManager.instance.ButtonPressed();
        float currentVol = _audioManager.volSFX;
        if (currentVol > 0)
            ChangeSFXVolume(currentVol-1);
        changedSFX.text = _audioManager.volSFX.ToString();
    }









    IEnumerator LoadNewScene()
    {
        float time = 0;
        Color wantedColor = _fadeScreen.color;
        isLoading = true;
        AsyncOperation asyncLoad  = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        asyncLoad.allowSceneActivation = false;
        
        yield return new WaitForSeconds(2f);

        _loadScreen.color = fullColor;
        yield return new WaitForSeconds(2f);
        _loadScreen.color = transparentColor;
        asyncLoad.allowSceneActivation = true;
        isLoading = false;
    }

    IEnumerator LoadSpecificScene(int scene)
    {
        float time = 0;
        Color wantedColor = _fadeScreen.color;
        isLoading = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false;

        while (time < fadeTime)
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
        
        float time;
        if (!_paused)
            time = 0;
        else
            time = fadeTime;
        Color wantedColor = _fadeScreen.color;
        
        while (time < fadeTime)
        {
            yield return new WaitForSeconds(fadeUpdateTime);

            wantedColor = Color.Lerp(blackColor, transparentColor, time);
            _fadeScreen.color = wantedColor;
            time += fadeUpdateTime;

            if(isLoading)
            {
                _fadeScreen.color = transparentColor;
                time = fadeTime + 1;
            }
        }
    }

    IEnumerator FadeOut()
    {
        float time;
        if (!_paused)
            time = 0;
        else
            time = fadeTime;
        _fadeScreen.color = transparentColor;
        //Color wantedColor = _fadeScreen.color;
        Color wantedColor = transparentColor;

        while (time < fadeTime)
        {
            yield return new WaitForSeconds(fadeUpdateTime);

            wantedColor = Color.Lerp(transparentColor, blackColor, time);
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
