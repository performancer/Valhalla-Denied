﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;                   //Allows us to use UI.

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 1f;                      //Time to wait before starting level, in seconds.

    public Font font;
    public Sprite block;
    public Sprite[] itemSprites;

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
    [HideInInspector] public bool paused;

    private SpriteManager spriteManager;
    private PlayerState playerState;

    private Text levelText;                                 //Text to display current level number.
    private GameObject levelImage, VDIMG;                          //Image to block out level as levels are being set up, background for levelText.
    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
    private int level = 1;                                  //Current level number
    private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
    private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
    private bool menu = false;
   

    public SpriteManager SpriteManager { get { return spriteManager; } }
    public PlayerState PlayerState { get { return playerState; } }
    public BoardManager BoardManager { get { return boardScript; } }
    public bool Escape { get { return menu; } set { menu = value; } }
 

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        spriteManager = new SpriteManager(font, block, itemSprites);
        playerState = new PlayerState();

        paused = false;

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    private string[] beginningThoughts = new string[]
    {
        "Where is my money..!",
        "I miss raiding...",
        "Am I the only one?",
        "What lies ahead..?",
        "Stop the voices..!",
        "By gods...",
        "To the darkest void...",
        "Is this before time?",
        "There are only monsters...",
    };

    private string[] middleThoughts = new string[]
    {
        "It never ends...",
        "I don't want to stay anylonger",
        "Guide me home...",
        "I hate stairs...",
        "Death awaits...",
        "Deeper and deeper...",
        "Rest...",
        "Bottomless...",
        "When does this end?"
    };

    private string[] endThoughts = new string[]
    {
        "Die die die!",
        "I am Death..!",
        "No defeat...",
        "Odin.. allfather",
        "Do you see me now?",
    };


    //Initializes the game for each level.
    void InitGame()
    {
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;
        VDIMG = GameObject.Find("VDIMG");
        //Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        VDIMG.SetActive(false); // This hides the Valhalla Denied logo after first level. 
        bool isBoss = level % 5 == 0 || level == 3;

        if (isBoss)
            levelText.text = "What is that menacing noise?";
        else if (level == 1 || level == 0)
        {
            VDIMG.SetActive(true);
            levelText.text = " ";
        }
        else if (level >= 0 && level <= 10 && level != 1)
            levelText.text = beginningThoughts[Random.Range(0, beginningThoughts.Length)];
        else if (level > 10 && level < 20)
            levelText.text = middleThoughts[Random.Range(0, middleThoughts.Length)];
        else
            levelText.text = endThoughts[Random.Range(0, endThoughts.Length)];


        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

            //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
            Invoke("HideLevelImage", levelStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level, isBoss);
    }


    //Hides black image used between levels
    public void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);
        VDIMG.SetActive(false);
        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }
   
    //Update is called every frame.
    void Update()
    {
        if (doingSetup || paused)
            return;

        MoveEnemies();
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void RemoveEnemyFromlist(Enemy script)
    {
        enemies.Remove(script);
    }

    //MainMenu is called when player press Esc. 
    public void MainMenu()
    {
        levelText.text = "Want to quit?\n Press Space to continue game \n Press Esc to quit \n Press Enter to Start new game";

        menu = true;
        paused = true;

        levelImage.SetActive(true);
    }
   
    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " levels, you died. \n Press Esc to quit \n Press Enter to start new game";
        menu = true;
        paused = true;
     
        //Enable black background image gameObject.
        levelImage.SetActive(true);

        //Disable this GameManager.
        enabled = false;
    }

    //Coroutine to move enemies in sequence.
    public void MoveEnemies()
    {
        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();
        }
    }

    public bool CheckIfTutorial()
    {
        if(level <= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetLevel()
    {
        return level;
    }

    public void RestartGame()
    {
        level = 0;
        playerState = new PlayerState();

        SoundManager.instance.musicSource.Play();
        enabled = true;
        menu = false;
        SceneManager.LoadScene(0);
    }
}


