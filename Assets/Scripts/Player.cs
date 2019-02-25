using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

public class Player : MovingObject
{
    public bool facingRight = true;             //Getting the player animations starting turn.
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
    public Text foodText;                       //UI Text to display current player food total.
    public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
    public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
    public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
    public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
    public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
    public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
    public AudioClip gameOverSound;             //Audio clip to play when player dies.

    public Image playerHpBar; //Player green health bar
    public Image playerManaBar;
    public Text manaText;
    public Text playerLevelText;

    #region Private Fields
    private Animator animator;                  //Used to store a reference to the Player's animator component. 
    private PlayerState state;
    private LoreScroll scroll;


    #endregion

    #region Properties
    public override int Hits
    {
        get { return state.Hits; }
        set
        {
            state.Hits = value;

            if (state.Hits > MaxHits)
                state.Hits = MaxHits;

            foodText.text = Hits + "/" + MaxHits;
        }
    }

    public int Gold
    {
        get { return state.Gold; }
        set { state.Gold = value; }
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
    #endregion

    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        state = GameManager.instance.PlayerState;

        MaxHits = 100 + (state.PlayerLevel -1) * 10;
        Damage = 10 + (state.PlayerLevel -1) * 5;

        UpdatePlayerHealthBar();
        UpdatePlayerManaBar();
        UpdatePlayerLevel();

        animator = GetComponent<Animator>();

        scroll = FindObjectOfType<LoreScroll>();

        base.Start();

        MoveDelay = TimeSpan.FromSeconds(0.3);

        UpdateCamera();
    }

    private void Update()
    {
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

        if (horizontal != 0 || vertical != 0)
            AttemptMove<MonoBehaviour>(horizontal, vertical);

        if (dpadhorizontal != 0 || dpadvertical != 0)
            AttemptMove<MonoBehaviour>(dpadhorizontal, dpadvertical);

        if (Input.GetKeyUp(KeyCode.X)) //Gain XP button for testing purposes
            GainXP(100);

        CheckFlip();
    }

    void CheckFlip() //Turning the player using Flip function.
    {
        float h = Input.GetAxis("Horizontal");
        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();
    }
    void Flip() //Mirror function for the player animation.
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
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
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        //Since the player has moved and lost food points, check if the game has ended.
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

            if(crit <= state.CritChance)
            {
                isCritical = true;
                critDmgModifier = 2;
            }

            if (Weapon != null)
            {
                enemy.LoseHits( ( (Damage + Weapon.Damage) * critDmgModifier) , isCritical);
            }
            else
            {
                enemy.LoseHits( (Damage * critDmgModifier), isCritical);
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
            other.gameObject.SetActive(false);
            state.Inventory.AddItem(new Food(20, "Apple", 10));
        }
        else if (other.tag == "Soda")
        {
            other.gameObject.SetActive(false);
            state.Inventory.AddItem(new Food(19, "Soda", 10));
        }
        else if(other.tag == "Scroll")
        {
            other.gameObject.SetActive(false);
            state.XpModifier += (float) 0.1;
            CreateFloatingText("+XPMOD", Color.white);
            scroll.ShowScroll();
        }
    }

    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public override void LoseHits(int dmg, bool isCrit)
    {
        int absorb = 0;

        if (Armor != null)
            absorb = (int)(dmg * Armor.Defense / 100.0f);

        Hits -= dmg - absorb;

        UpdatePlayerHealthBar();

   
        CreateFloatingText(Convert.ToString(dmg - absorb), Color.yellow);
        
        animator.SetTrigger("playerHit");

        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (Hits <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
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

    private void UpdatePlayerHealthBar()
    {
        playerHpBar.fillAmount = state.Hits / (float)MaxHits; //Reduces the green "fill" on the red HpBackground
        foodText.text = Hits + "/" + MaxHits;
    }

    
    public void UpdatePlayerManaBar()
    {
        playerManaBar.fillAmount = state.CurrentXp / (float)state.MaxXp; //Increases the blue "fill"
        manaText.text = state.CurrentXp + "/" + state.MaxXp;
    }
    

    public void UpdatePlayerLevel()
    {
        playerLevelText.text = "LVL:" + state.PlayerLevel;
    }


    public void GainXP(float xp)
    {
        xp = xp * state.XpModifier;

        state.CurrentXp += Mathf.Floor(xp);

        if (state.CurrentXp < state.MaxXp)
        {
            CreateFloatingText(Convert.ToString(xp), Color.white);
        }
        else
        {
            LevelUp();
        }
        UpdatePlayerManaBar();
    }

    public void LevelUp()
    {
        state.OverflowXp = state.CurrentXp - state.MaxXp;
        state.PlayerLevel++;
        CreateFloatingText("LVL UP!", Color.blue);
        state.MaxXp = 100 * state.PlayerLevel * Mathf.Pow(state.PlayerLevel, 0.5f);
        state.MaxXp = Mathf.Floor(state.MaxXp);
        state.CurrentXp = Mathf.Floor(0 + state.OverflowXp);
        

        Damage += 5;
        MaxHits += 10;
        Hits += 10;
        state.CritChance += (float)2.5;

        UpdatePlayerHealthBar();
        UpdatePlayerLevel();
        UpdatePlayerManaBar();
    }


}


