using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public MovingObject testi;
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

        Remove();
    }
}
