using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    private int defense;

    public int Defense { get { return defense; } }

    public Armor(string name, int defense) : base(name)
    {
        this.defense = defense;
    }

    public override bool Use(Player player)
    {
        player.Armor = this;
        return false;
    }
}
