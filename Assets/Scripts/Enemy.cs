using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI; //For HealthBar UI


//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
                    
    public int PlayerDamage;                            //The amount of food points to subtract from the player when attacking.
    public AudioClip AttackSound1;                      //First of two audio clips to play when attacking the player.
    public AudioClip AttackSound2;                      //Second of two audio clips to play when attacking the player.

    private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    private Transform target;                           //Transform to attempt to move toward each turn.

    public HpBar hpBar2; //public koska EnemyPrefab

    //Start overrides the virtual Start function of the base class.
    protected override void Start()
    {
        MaxHits = Hits = 100;
        Damage = PlayerDamage;

        GameManager.instance.AddEnemyToList(this);

        animator = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

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
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (target.position.x < transform.position.x)
        {
            //face left
            transform.localScale = new Vector3(1, 1, 1);
        }
    }



    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LoseHits(Damage);

        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomizeSfx(AttackSound1, AttackSound2);
    }




    public override void LoseHits(int dmg)
    {
        base.LoseHits(dmg);

        hpBar2.HpBarFilled.fillAmount = Hits / (float)MaxHits; //Reduces the green "fill" on the red HpBackground

        CreateFloatingText(dmg, Color.red);

        if (Hits <= 0)
        {
            hpBar2.DestroyBar();
            GameManager.instance.RemoveEnemyFromlist(this);
            Destroy(gameObject);
        }
    }
 
}



