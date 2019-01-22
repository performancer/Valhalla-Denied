using System;
using UnityEngine;
using System.Collections;


//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
    public int PlayerDamage;                            //The amount of food points to subtract from the player when attacking.
    public AudioClip AttackSound1;                      //First of two audio clips to play when attacking the player.
    public AudioClip AttackSound2;                      //Second of two audio clips to play when attacking the player.

    private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    private Transform target;                           //Transform to attempt to move toward each turn.

    //Start overrides the virtual Start function of the base class.
    protected override void Start()
    {
        base.Start();

        Damage = PlayerDamage;

        GameManager.instance.AddEnemyToList(this);

        animator = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
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

        if (Hits <= 0)
        {
            GameManager.instance.RemoveEnemyFromlist(this);
            Destroy(gameObject);
        }
    }
}

