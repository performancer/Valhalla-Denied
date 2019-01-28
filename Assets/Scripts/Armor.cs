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

    public override bool Use(Player player)
    {
        player.Armor = this;
        return false;
    }
}
