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
        //Cannot eat food if HP is full
        if (from.Hits >= from.MaxHits)
            return;

        from.Hits += health;

        Remove();
    }
}
