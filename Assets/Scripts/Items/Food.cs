using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Food : Item
{
    int health;

    public Food (int id, string name, int health) : base(id, name)
    {
        this.health = health;
    }

    public override void Use(MovingObject from)
    {
        if (from.Hits >= from.MaxHits)
            return;

        from.Hits += health;

        
        Player player = GameObject.FindObjectOfType<Player>();
        player.UpdateHpBar();
        player.CreateFloatingText("+"+health.ToString(), Color.green);
        

        if (ID == 0) //eat sounds
            SoundManager.instance.RandomizeSfx(2, 3);
        else if (ID == 1) //drink sounds
            SoundManager.instance.RandomizeSfx(4, 5);


        Remove();
    }
}
