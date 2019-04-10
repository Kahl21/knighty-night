using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class LoadingAndSavingTool
{

    //call whenever collectible collected after collectibleReference completed
    public static void Save()
    {
        //create a binary formatter so the file can be translated so the computer can save it
        BinaryFormatter bf = new BinaryFormatter();
        //get the file and open it
        FileStream file = File.Create(Application.persistentDataPath + "/saveState.dat");

        //-----------
        //this is where the savings happen

        //make a gamedata
        GameData data = new GameData();



        //make saves = saved CompletedLevels
        data._unlockedLevels = GameManager._themesUnlocked;
        data.lastLevel = GameManager._lastLevelIndex;
        //------------

        //put data into file
        bf.Serialize(file, data);
        //close the file
        file.Close();
        Debug.Log("File Saved");
    }

    //call when player starts game
    public static void Load()
    {
        //if file exists
        if (File.Exists(Application.persistentDataPath + "/saveState.dat"))
        {
            //make a bf to translate it
            BinaryFormatter bf = new BinaryFormatter();
            //open the file
            FileStream file = File.Open(Application.persistentDataPath + "/saveState.dat", FileMode.Open);
            //make a gameData class instance to hold the data
            GameData data = (GameData)bf.Deserialize(file); //need to cast file (which computer reads as an object) to a GameData
            //close the file 
            file.Close();
            Debug.Log("Save Loaded");
            //----------------------
            //this is where the loadings happen

            /*for (int index = 0; index < data.level1Collectibles.Length; index++)
            {
                if(data.level1Collectibles[index] == true)
                {
                    Debug.Log(data.level1Collectibles[index]);
                }
            }
            

            Debug.Log("load: " + data.level1Collectibles.Length);
            */

            //Load levels Completed
            GameManager._themesUnlocked = data._unlockedLevels;
            GameManager._lastLevelIndex = data.lastLevel;
            //levelManager.firstOpen = data.firstOpen;
            //----------------------

        }
        else
        {
            //Create a new Save
            GameManager._themesUnlocked = GameManager.Instance.CreateSave();
            Save();
            Debug.Log("New Save Created");
        }

    }

    public static void DeleteSave()
    {
        //if save file exists
        if (File.Exists(Application.persistentDataPath + "/saveState.dat"))
        {
            //delete it
            File.Delete(Application.persistentDataPath + "/saveState.dat");
        }
    }

}

[Serializable]
class GameData
{
    public bool[] _unlockedLevels;
    public int lastLevel;
    //public bool firstOpen;
}


//GuideMeHome Levelmanager Script
/*
 public static bool[] levelsCompleted;
    public static bool firstOpen = true;
    public bool skipTutorial;
    public bool deleteSave = false;
    //string path = "Assets/save.txt";
    //string save;
    //string[] saves;
    public Sprite lockedButton;
    public Sprite unLockedButton;
    GameObject[] buttons;
    public VideoPlayer videoPlayer;
    bool playing = false;
    //TextAsset txtPath;
    //public TextAsset txtSaves;
    // Use this for initialization
    private void Awake()
    {
            DontDestroyOnLoad(gameObject);
        /*
#if UNITY_ANDROID
        path = Application.persistentDataPath + "/save.txt";
        if (Application.dataPath + "/save.txt" == null)
        {
            File.Create(Application.persistentDataPath + "/save.txt");
            File.WriteAllLines(Application.dataPath + "/save.txt", new string[15] {"true", "false", "false", "false"
            , "false", "false", "false", "false", "false", "false", "false", "false", "false", "false", "false"});
        }
#endif
    //
        //LoadAndSaveTool.Load();
        if (deleteSave && firstOpen == true)
        {
            Debug.Log("delete Save");
            LoadAndSaveTool.DeleteSave();
            LoadAndSaveTool.Load();
        }
        LoadAndSaveTool.Load();
        if (GameObject.FindGameObjectsWithTag("levelManager").Length > 1)
        {
            Destroy(this.gameObject);
        }
        
    }

    void Start()
{
    if (skipTutorial)
    {
        firstOpen = false;
    }
    StartCoroutine(playOpeningScene());
}

private void Update()
{
    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LevelSelect"))
    {
        unlockScenes();
    }

}

private void unlockScenes()
{
    buttons = getButtons();
    /*
    for (int i = 0; i < saves.Length; i++)
    {
        //Debug.Log(saves[i]);
        int reference = (i - (saves.Length -1)) * -1;
        if (saves[i] == "true")
        {
            buttons[reference].GetComponent<Image>().sprite = unLockedButton;
            buttons[reference].GetComponent<Button>().enabled = true;
            buttons[reference].GetComponent<Button>().interactable = true;
        }
        else if (saves[i] == "false")
        {
            buttons[reference].GetComponent<Image>().sprite = lockedButton;
            buttons[reference].GetComponent<Button>().enabled = false;
            buttons[reference].GetComponent<Button>().interactable = false;
        }
    }
    //
    for (int i = 0; i < levelsCompleted.Length; i++)
    {
        //Debug.Log(saves[i]);
        int reference = (i);
        Debug.Log(reference);
        if (levelsCompleted[i] == true)
        {
            Debug.Log(buttons[reference].name + " Is Unlocked");
            buttons[reference].GetComponent<Image>().sprite = unLockedButton;
            buttons[reference].GetComponent<Button>().enabled = true;
            buttons[reference].GetComponent<Button>().interactable = true;
        }
        else if (levelsCompleted[i] == false)
        {
            Debug.Log(buttons[reference].name + " Is locked");
            buttons[reference].GetComponent<Image>().sprite = lockedButton;
            buttons[reference].GetComponent<Button>().enabled = false;
            buttons[reference].GetComponent<Button>().interactable = false;
        }
    }
}

private GameObject[] getButtons()
{
    GameObject[] buttons;
    buttons = GameObject.FindGameObjectsWithTag("button");
    for (int i = 0; i < buttons.Length; i++)
    {
        buttons[i] = GameObject.Find("ButtonLevel" + i.ToString());
    }

    return buttons;
}

public void saveLevel(int levelIndex)
{
    levelsCompleted[levelIndex] = true;
    LoadAndSaveTool.Save();
}

public void unlockAllLevels()
{
    for (int i = 0; i < 10; i++)
    {
        levelsCompleted[i] = true;
        print("Level " + i + " is " + levelsCompleted[i]);
    }
}

public void resetSave()
{
    LoadAndSaveTool.DeleteSave();
}

public bool[] createSave()
{
    bool[] newSave = new bool[10];
    for (int i = 0; i < newSave.Length; i++)
    {
        Debug.Log("creating new save");
        if (i == 0)
        {
            Debug.Log(i + "is unlocked");
            newSave[i] = true;
        }
        else
        {
            Debug.Log(i + "Is Locked");
            newSave[i] = false;
        }
    }
    firstOpen = true;
    return newSave;
}

IEnumerator playOpeningScene()
{
    int seconds = 0;
    while (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
    {
        VideoPlayer vPlayer;
        if (seconds == 10)
        {
            Debug.Log("Play Video");
            vPlayer = Instantiate(videoPlayer);
            vPlayer.targetCamera = Camera.main;
            vPlayer.Play();
            yield return new WaitForSeconds(52f);
            Destroy(vPlayer);
            seconds = -10;
        }

        yield return new WaitForSeconds(1f);
        seconds++;
    }


}
*/

#region OpenMe

/*
 * 
 *     .sS$$$$$$$$$$$$$$Ss.
     .$$$$$$$$$$$$$$$$$$$$$$s.
     $$$$$$$$$$$$$$$$$$$$$$$$S.
     $$$$$$$$$$$$$$$$$$$$$$$$$$s.
     S$$$$'        `$$$$$$$$$$$$$
     `$$'            `$$$$$$$$$$$.
      :               `$$$$$$$$$$$
     :                 `$$$$$$$$$$
  .====.  ,=====.       $$$$$$$$$$
.'      ~'       ".    s$$$$$$$$$$
:       :         :=_  $$$$$$$$$$$
`.  ()  :   ()    ' ~=$$$$$$$$$$$'
  ~====~`.      .'    $$$$$$$$$$$
   .'     ~====~     sS$$$$$$$$$'
   :      .         $$$$$' $$$$
 .sS$$$$$$$$Ss.     `$$'   $$$'
$$$$$$$$$$$$$$$s         s$$$$
$SSSSSSSSSSSSSSS$        $$$$$
     :                   $$$$'
      `.                 $$$'
        `.               :
         :               :
         :              .'`.
        .'.           .'   :
       : .$s.       .'    .'
       :.S$$$S.   .'    .'
       : $$$$$$`.'    .'
          $$$$   `. .'
                   `
*/
#endregion
