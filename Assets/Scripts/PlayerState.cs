using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{   
    private int hits;
    private int gold;

    private List<Item> items;
    private Armor armor;
    private Weapon weapon;

    public int Hits
    {
        get { return hits; }
        set { hits = value; }
    }
    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

    public List<Item> Items
    {
        get { return items; }
        set { items = value; }
    }

    public Armor Armor
    {
        get { return armor; }
        set { armor = value; }
    }

    public Weapon Weapon
    {
        get { return weapon; }
        set { weapon = value; }
    }

    public PlayerState()
    {
        hits = 100;
        gold = 0;

        items = new List<Item>();

        items.Add(new Armor("Iron Helmet", 10));
        items.Add(new Weapon("Bastard Sword", 20));
    }
}
