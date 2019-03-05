using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoreScroll : MonoBehaviour
{

    public GameObject scrollBox;
    public Text scrollText;
    public Text scrollTextContinue;

    private bool scrollActive;
    private GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        scrollBox.SetActive(false);
        scrollActive = false;
        manager = GameManager.instance;

        if (manager.CheckIfTutorial() == true)
        {
            ShowScroll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(scrollActive == true && manager.paused == true && (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.JoystickButton0)))
        {
            scrollBox.SetActive(false);
            scrollActive = false;
            manager.paused = false;
        }
    }

    public void ShowScroll()
    {
        scrollBox.SetActive(true);
        scrollActive = true;

        bool isTutorial;

        if (manager.CheckIfTutorial() == true)
            isTutorial = true;
        else
            isTutorial = false;

        if (isTutorial == true)
            TutorialScroll(manager.GetLevel());
        else
            scrollText.text = loreTexts[Random.Range(0, loreTexts.Length)];


        if (DetectXboxController() == true)
        {
            scrollTextContinue.text = "Press A to Continue";
        } else
        {
            scrollTextContinue.text = "Press E to Continue";
        }

        manager.paused = true;
    }

    private string[] loreTexts = new string[]
    {
        "Then is fulfilled Hlín's " +
        "second sorrow, " +
        "when Óðinn goes " +
        "to fight with the wolf, " +
        "and Beli's slayer, " +
        "bright, against Surtr. " +
        "Then shall Frigg's " +
        "sweet friend fall. ",

        "Hearing I ask from the holy races, "+
        "From Heimdall's sons, both high and low; "+
        "Thou wilt, Valfather, that well I relate " +
        "Old tales I remember of men long ago. " ,

        "What sort of dream is that, Odin? I dreamed I rose up before dawn to clear up Val-hall for slain people. I aroused the Einheriar, bade them get up to strew the benches, clean the beer-cups, the valkyries to serve wine for the arrival of a prince.",

        "Glad Realm the fifth is called, "+
        "where, gleaming with gold, "+
        "the Hall of the Slain Men stretches "+
        "into space, "+
        "and there the Hidden One "+
        "each day elects "+
        "the men who met death by weapons." ,

        "All the Einheriar fight in Odin's courts every day; they choose the slain and ride from battle; then they sit more at peace together. ",

        "If all things in the world, alive or dead, weep for him, then he will be allowed to return to the Æsir. If anyone speaks against him or refuses to cry, then he will remain with Hel.",

        "Now my course is tough: Death, close sister of Odin's enemy stands on the ness: with resolution and without remorse I will gladly await my own." ,

    };

    private void TutorialScroll(int level)
    {
        string movementText;
        string inventoryText;

        if (DetectXboxController() == true)
        {
            movementText = "Joysticks and the D-pad";
            inventoryText = "START";
        }
        else
        {
            movementText = "WASD and the arrow keys";
            inventoryText = "I";
        }

        if (level == 1)
        {
            scrollText.text = "Welcome to Valhalla Denied. You can move by "+movementText+". You can destroy walls by moving on them.\n\nTake up your weapon and armor of your past life from the ground.\nPress "+inventoryText+" to access your inventory.\n\nGo down the stairs to go deeper...";
        } else if (level == 2)
        {
            scrollText.text = "On this floor is your first enemy. Try not to die.\n\nHeal up by eating food. \n\nYou are on your own now.\n\nAre you able to escape Hell and go to Valhalla?";
        }
    }

    private bool DetectXboxController()
    {
        string[] names = Input.GetJoystickNames();

        bool isXboxController = false;

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == "Controller (Xbox One For Windows)")
            {
                isXboxController = true;
            }
            else
            {
                isXboxController = false;
            }
        }

        if (isXboxController == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
