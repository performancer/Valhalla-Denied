using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{   
    private int hits;
    private int gold;

    private Inventory inventory;
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

    public Inventory Inventory
    {
        get { return inventory; }
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

        inventory = new Inventory(this);
    }
}
