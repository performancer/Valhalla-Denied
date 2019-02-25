using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Food : Item
{
    int health;

   //private Player player;

    void Start()
    {
         //player = FindObjectOfType<Player>();
    }

    public Food (int id, string name, int health) : base(id, name)
    {
        this.health = health;
    }

    public override void Use(MovingObject from)
    {
        
        if (from.Hits >= from.MaxHits)
            return;

        from.Hits += health;
        //player.UpdatePlayerHealthBar();
        //player.CreateFloatingText(Convert.ToString(health), Color.green);

        if (ID == 20) //eat sounds
            SoundManager.instance.RandomizeSfx(2, 3);
        else if (ID == 19) //drink sounds
            SoundManager.instance.RandomizeSfx(4, 5);


        Remove();
    }
}
