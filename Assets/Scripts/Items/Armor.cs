using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    private int defense;

    public int Defense { get { return defense; } }

    public Armor(int id, string name, int defense) : base(id, name)
    {
        this.defense = defense;
    }

    public override void Use(MovingObject from)
    {
        if (from is Player)
        {
            Player player = from as Player;
            player.Armor = this;
        }
    }
}
