using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    int health;

    public Food (string name, int health) : base(name)
    {
        this.health = health;
    }

    public override bool Use(Player player)
    {
        //Cannot eat food if HP is full
        if (player.Hits >= player.MaxHits)
            return false;

        player.Hits += health;
        return true;
    }
}
