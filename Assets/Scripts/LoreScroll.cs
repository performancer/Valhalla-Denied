using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoreScroll : MonoBehaviour
{

    public GameObject scrollBox;
    public Text scrollText;
    private bool scrollActive;
    //private GameManager scrollManager;


    // Start is called before the first frame update
    void Start()
    {
        scrollBox.SetActive(false);
        scrollActive = false;
        //scrollManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(scrollActive == true && Input.GetKeyUp(KeyCode.U) /*&& scrollManager.paused == true*/)
        {
            scrollBox.SetActive(false);
            scrollActive = false;
            //scrollManager.paused = false;
        }
    }

    public void ShowScroll()
    {
        scrollBox.SetActive(true);
        scrollActive = true;
        scrollText.text = loreTexts[Random.Range(0, loreTexts.Length)];
        //scrollManager.paused = true;
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
}
