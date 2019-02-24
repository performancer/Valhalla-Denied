using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState
{
    private int hits;
    private int gold;

    private float currentxp;
    private float maxxp;
    private float overflowxp;
    private int playerLevel;
    private float critchance;

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

    public float CurrentXp
    {
        get { return currentxp; }
        set { currentxp = value; }
    }

    public float MaxXp
    {
        get { return maxxp; }
        set { maxxp = value; }
    }

    public float OverflowXp
    {
        get { return overflowxp; }
        set { overflowxp = value; }
    }

    public float CritChance
    {
        get { return critchance; }
        set { critchance = value; }
    }

    public int PlayerLevel
    {
        get { return playerLevel; }
        set { playerLevel = value; }
    }

    public Inventory Inventory
    {
        get { return inventory; }
    }

    public Armor Armor
    {
        get
        {
            return armor;
        }
        set
        {
            armor = value;
            RefreshEquipment(GameObject.Find("ArmorSlot"), armor);
        }
    }

    public Weapon Weapon
    {
        get
        {
            return weapon;
        }
        set
        {
            weapon = value;
            RefreshEquipment(GameObject.Find("WeaponSlot"), weapon);
        }
    }



    public PlayerState()
    {
        hits = 100;
        gold = 0;

        maxxp = 100;
        playerLevel = 1;
        critchance = 0;

    inventory = new Inventory(this);

    }

    public void RefreshEquipment(GameObject slot, BaseEquipment equipment)
    {
        slot.transform.DestroyChildren();

        if (equipment == null)
            return;

        SpriteManager sprites = GameManager.instance.SpriteManager;
        SpriteManager.CreateImage(slot.transform, sprites.GetSprite(equipment.ID));
        SpriteManager.CreateText(slot.transform, 16, new Vector3(40, -40, 0), true).text = equipment.GetValue().ToString();
    }
}
