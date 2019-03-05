using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI; //For HealthBar UI


//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{                
    public int PlayerDamage;                            //The amount of food points to subtract from the player when attacking. 
    private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    private Transform target;                           //Transform to attempt to move toward each turn.

    public HpBar hpBar2; //public koska EnemyPrefab

    public int experienceValue;
 
    private Player playerReference;

    private BoardManager board;

    //Start overrides the virtual Start function of the base class.
    protected override void Start()
    {
        board = GameManager.instance.BoardManager;
        GameManager manager = GameManager.instance;
       

        if (board.IsBossRoom)
        {
            MaxHits = Hits = 100 + 50 + manager.GetLevel() * 20;
            Damage = PlayerDamage + manager.GetLevel() * 3;
            experienceValue = experienceValue * 4 + manager.GetLevel() * 50;
            PoisonDamage = PoisonDamage + manager.GetLevel();
        }
        else if (board.IsBossRoom && manager.CheckIfTutorial())
        {
            MaxHits = Hits = 100 + manager.GetLevel() * 20;
            Damage = PlayerDamage + manager.GetLevel();
            experienceValue = experienceValue * 2 + manager.GetLevel() * 50;
        }
        else
        {
            Damage = PlayerDamage + manager.GetLevel();
            MaxHits = Hits = 100 + manager.GetLevel() * 5;
        }

        GameManager.instance.AddEnemyToList(this);

        animator = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        playerReference = FindObjectOfType<Player>();

        

        base.Start();
    }

    public void MoveEnemy()
    {
        if (LastMove + MoveDelay > DateTime.Now)
            return;

        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    private void FixedUpdate() // turning the enemy face to the direction it's moving.
    {
        if (target.position.x > transform.position.x)
        {
            //face right
            transform.localScale = new Vector3(-(Math.Abs(transform.localScale.x)), transform.localScale.y, transform.localScale.z);
        }
        else if (target.position.x < transform.position.x)
        {
            //face left
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }



    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        bool crit = false;
        hitPlayer.LoseHits(Damage, crit, isPoison, PoisonDamage);

        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomizeSfx(7,8);
    }

    public override void LoseHits(int dmg, bool isCrit, bool isPoison, int poisondmg)
    {
        base.LoseHits(dmg,isCrit,isPoison, poisondmg);

        //Hits -= dmg;

        if (isPoison == true && !IsPoisoned)
            StartCoroutine("Poison", poisondmg);

        UpdateHpBar();

        if(isCrit == true)
        {
            CreateFloatingText(dmg.ToString(), Color.cyan);
        }
        else
        {
            CreateFloatingText(dmg.ToString(), Color.red);
        }

        if (Hits <= 0)
            OnDeath();
    }

    public virtual void OnDeath()
    {
        if (board.IsBossRoom)
        {
            board.CreateRandomExit();
        }

        if(board.IsBossRoom && GameManager.instance.CheckIfTutorial() == false)
        {
            board.CreateSword();
            board.CreateArmor();
        }

        playerReference.GainXP(experienceValue);
        hpBar2.DestroyBar();
        GameManager.instance.RemoveEnemyFromlist(this);
        Destroy(gameObject);
    }

    public override void UpdateHpBar()
    {
        hpBar2.HpBarFilled.fillAmount = Hits / (float)MaxHits; //Reduces the green "fill" on the red HpBackground
    }

}



