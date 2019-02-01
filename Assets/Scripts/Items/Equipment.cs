using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEquipment : Item
{
    public BaseEquipment(int id, string name) : base(id, name)
    {

    }

    public override void Use(MovingObject from)
    {
        if (from is Player)
            Equip(from as Player);
    }

    public abstract bool Equip(Player player);
    public abstract int GetValue();
}

public class Armor :  BaseEquipment
{
    private int defense;

    public int Defense { get { return defense; } }

    public Armor(int id, string name, int defense) : base(id, name)
    {
        this.defense = defense;
    }

    public override bool Equip(Player player)
    {
        if (player.Armor != this)
            player.Armor = this;
        else
            player.Armor = null;

        return true;
    }

    public override int GetValue()
    {
        return defense;
    }
}

public class Weapon : BaseEquipment
{
    private int damage;

    public int Damage { get { return damage; } }

    public Weapon(int id, string name, int damage) : base(id, name)
    {
        this.damage = damage;
    }

    public override bool Equip(Player player)
    {
        if (player.Weapon != this)
            player.Weapon = this;
        else
            player.Weapon = null;

        return true;
    }

    public override int GetValue()
    {
        return damage;
    }
}
