﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
    public Text hpText;                       //UI Text to display current player hitpoint total.

    public Image playerHpBar; //Player green health bar
    public Image playerXpBar;
    public Text xpText;
    public Text playerLevelText;

    #region Private Fields
    private bool gameover = false;
    private Animator animator;                  //Used to store a reference to the Player's animator component. 
    private PlayerState state;
    private LoreScroll scroll;
    private PowerUp powerup;
    #endregion

    #region Properties
    public override int Hits
    {
        get { return state.Hits; }
        set
        {
            state.Hits = value;

            if (state.Hits > state.MaxHits)
                state.Hits = state.MaxHits;
                MaxHits = state.MaxHits;

            hpText.text = Hits + "/" + MaxHits;
        }
    }

    public Armor Armor
    {
        get { return state.Armor; }
        set { state.Armor = value; }
    }

    public Weapon Weapon
    {
        get { return state.Weapon; }
        set { state.Weapon = value; }
    }

    public GameManager Manager
    {
        get { return GameManager.instance; }
    }

    
    #endregion

    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        state = GameManager.instance.PlayerState;
        state.Refresh();

        MaxHits = 100 + (state.PlayerLevel -1) * 10;
        Damage = 10 + (state.PlayerLevel - 1) * 5;

        CritBuff = 0;

        UpdateHpBar();
        UpdatePlayerXpBar();
        UpdatePlayerLevel();

        animator = GetComponent<Animator>();

        scroll = FindObjectOfType<LoreScroll>();

        powerup = FindObjectOfType<PowerUp>();

        base.Start();

        MoveDelay = TimeSpan.FromSeconds(0.3);

        UpdateCamera();
        
    }

    private void Update()
    {
        if (!Manager.paused && Input.GetKeyUp(KeyCode.Escape)) //Opens Exit window when Esc is pressed
        {
            Manager.MainMenu();
            return;
        }
        else if (Manager.Escape)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            else if (Input.GetKeyUp(KeyCode.Return)) //On GameOver and Escape window, pressing Enter starts new game.
            {
                Manager.RestartGame();
            }
            else if (gameover == false && Input.GetKeyUp(KeyCode.Space)) //Let's player to continue game after pressing Esc
            {
                Manager.paused = false;
                Manager.Escape = false;
                Manager.HideLevelImage();
            }

            return;
        }

        if (LastMove + MoveDelay > DateTime.Now)
            return;

        state.Inventory.Update(this);

        if (GameManager.instance.paused)
            return;

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.

        int dpadhorizontal = 0;
        int dpadvertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        dpadhorizontal = (int)(Input.GetAxisRaw("DpadHorizontal")); //DpadHorizontal
        dpadvertical = (int)(Input.GetAxisRaw("DpadVertical")); //DpadVertical

        
        if (horizontal != 0)
            vertical = 0;

        if (dpadhorizontal != 0)
            dpadvertical = 0;

        CheckFlip();

        if (horizontal != 0 || vertical != 0)
            AttemptMove<MonoBehaviour>(horizontal, vertical);

        if (dpadhorizontal != 0 || dpadvertical != 0)
            AttemptMove<MonoBehaviour>(dpadhorizontal, dpadvertical);

        CheckIfGameOver();
    }

    void CheckFlip() //Turning the player using Flip function.
    {
        float h = Input.GetAxis("Horizontal");
        if (h > 0 && Flipped)
            Flip();
        else if (h < 0 && !Flipped)
            Flip();
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove<T>(xDir, yDir);

        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;

        //If Move returns true, meaning Player was able to move into an empty space.
        if (Move(xDir, yDir, out hit))
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            SoundManager.instance.RandomizeSfx(0, 1);
        }

        CheckIfGameOver();
    }
    
    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove<T>(T component)
    {
        if (component is Wall)
        {
            //Set hitWall to equal the component passed in as a parameter.
            Wall hitWall = component as Wall;

            //Call the DamageWall function of the Wall we are hitting.
            hitWall.DamageWall(wallDamage);

            //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
            animator.SetTrigger("playerChop");
        }
        else if (component is Enemy)
        {
            Enemy enemy = component as Enemy;

            

            int crit = Random.Range(1, 100);
            int critDmgModifier = 1;
            bool isCritical = false;           

            if(crit <= state.CriticalHitChance + CritBuff)
            {
                
                isCritical = true;
                critDmgModifier = 2;
            }

            SoundManager.instance.RandomizeSfx(9, 10);
            CheckIfGameOver();


            if (Weapon != null)
            {
                enemy.LoseHits(((Damage + Weapon.Damage) * critDmgModifier), isCritical, isPoison, PoisonDamage);
            }
            else
            {
                enemy.LoseHits((Damage * critDmgModifier), isCritical, isPoison, PoisonDamage);
            }

            animator.SetTrigger("playerChop");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            if (state.Inventory.AddItem(this, new Food((int)ItemSprite.Salmon, "Salmon", 10)))
                other.gameObject.SetActive(false);
        }
        else if (other.tag == "Mead")
        {
            if(state.Inventory.AddItem(this, new Food((int)ItemSprite.Mead, "Mead", 20)))
                other.gameObject.SetActive(false);
        }
        else if (other.tag == "Sword")
        {
            GameManager manager = FindObjectOfType<GameManager>();

            int sworddmg;
            if(manager.CheckIfTutorial() == false)
            {
                sworddmg = 10 + Random.Range(1, 10) + manager.GetLevel();

            } else
            {
                sworddmg = 5;
            }
            if (state.Inventory.AddItem(this, new Weapon((int)ItemSprite.IronSword, "Iron Sword DMG: " + (sworddmg), sworddmg)))
                other.gameObject.SetActive(false);
        }
        else if (other.tag == "LegendarySword")
        {
            GameManager manager = FindObjectOfType<GameManager>();
            int sworddmg = 10 + Random.Range(15, 30) + manager.GetLevel();

            if (state.Inventory.AddItem(this, new Weapon((int)ItemSprite.LegendaryVikingSword, "Legendary Viking Sword DMG: " + (sworddmg), sworddmg)))
                other.gameObject.SetActive(false);
        }
        else if (other.tag == "Armor")
        {
            int armorvalue;
            GameManager manager = FindObjectOfType<GameManager>();
            if (manager.CheckIfTutorial() == false)
            {
                armorvalue = 10 + Random.Range(1, 10) + manager.GetLevel();
            }
            else
            {
                armorvalue = 5;
            }
            if (state.Inventory.AddItem(this, new Armor((int)ItemSprite.IronArmor, "Iron Armor AV: " + (armorvalue), armorvalue)))
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "LegendaryArmor")
        {
            GameManager manager = FindObjectOfType<GameManager>();
            int armorvalue = 10 + Random.Range(15, 30) + manager.GetLevel();

            if (state.Inventory.AddItem(this, new Armor((int)ItemSprite.LegendaryVikingArmor, "Legendary Viking Armor AV: " + (armorvalue), armorvalue)))
                other.gameObject.SetActive(false);
        }
        else if(other.tag == "Scroll")
        {
            float experiencePointModifierGain = (float)0.1;
            other.gameObject.SetActive(false);
            state.ExperienceGainModifier += experiencePointModifierGain;
            CreateFloatingText("+" + experiencePointModifierGain * 100+ "% XPMOD", Color.white);
            scroll.ShowScroll();
        }
        else if (other.tag == "PowerUp")
        {
            CreateFloatingText("RAGE", Color.red);
        }
    }

    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public override void LoseHits(int dmg, bool isCrit, bool isPoison, int poisondmg)
    {
        int absorb = 0;

        if (Armor != null)
            absorb = (int)(dmg * Armor.Defense / 100.0f);

        Hits -= dmg - absorb;

        if (isPoison == true && !IsPoisoned) 
            StartCoroutine("Poison",poisondmg);
           
        UpdateHpBar();
 
        CreateFloatingText(Convert.ToString(dmg - absorb), Color.yellow);
        
        animator.SetTrigger("playerHit");
    }

    private void CheckIfGameOver()
    {
        if (Hits <= 0)
        {
            SoundManager.instance.PlaySingle(6);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
            gameover = true;
        }
    }

    protected override void OnMovement()
    {
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        GameObject camera = GameObject.Find("Main Camera");
        Transform transform = camera.transform;

        Vector3 position = this.transform.position;
        transform.position = new Vector3(position.x, position.y, -10);
    }


    
    public void UpdatePlayerXpBar()
    {
        playerXpBar.fillAmount = state.CurrentExperience / (float)state.MaximumExperience; //Increases the blue "fill"
        xpText.text = state.CurrentExperience + "/" + state.MaximumExperience;
    }
    

    public void UpdatePlayerLevel()
    {
        playerLevelText.text = "LVL:" + state.PlayerLevel;
    }

    public void GainXP(float xp)
    {

        xp = Mathf.Floor(xp * state.ExperienceGainModifier);

        state.CurrentExperience += xp;

        if (state.CurrentExperience < state.MaximumExperience)
        {
            CreateFloatingText(Convert.ToString(xp), Color.white);
        }
        else
        {
            LevelUp();
            scroll.ShowLevelUpScroll();
        }
        UpdatePlayerXpBar();
    }

    public void LevelUp()
    {
        float overflowxp;
        overflowxp = state.CurrentExperience - state.MaximumExperience;
        state.PlayerLevel++;
        CreateFloatingText("LVL UP!", Color.blue);
        state.MaximumExperience = 100 * state.PlayerLevel * Mathf.Pow(state.PlayerLevel, 0.5f);
        state.MaximumExperience = Mathf.Floor(state.MaximumExperience);
        state.CurrentExperience = Mathf.Floor(0 + overflowxp);

        state.Damage += 5;
        Damage = state.Damage;

        state.CriticalHitChance += (float)1.0;
        state.MaxHits += 5;
        MaxHits = state.MaxHits;
        Hits = MaxHits;

        UpdateHpBar();
        UpdatePlayerLevel();
        UpdatePlayerXpBar();
    }

    public override void UpdateHpBar()
    {
        playerHpBar.fillAmount = state.Hits / (float)state.MaxHits; //Reduces the green "fill" on the red HpBackground
        hpText.text = state.Hits + "/" + state.MaxHits;
    }


}


