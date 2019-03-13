using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState
{
    private int hits;
    private int maxhits;
    private int damage;

    private float currentExperience;
    private float maximumExperience;
    private int playerLevel;
    private float criticalHitChance;
    private float experienceGainModifier;

    private Inventory inventory;
    private Armor armor;
    private Weapon weapon;

    public int Hits
    {
        get { return hits; }
        set { hits = value; }
    }
    public int MaxHits
    {
        get { return maxhits; }
        set { maxhits = value; }
    }


    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public float CurrentExperience
    {
        get { return currentExperience; }
        set { currentExperience = value; }
    }

    public float MaximumExperience
    {
        get { return maximumExperience; }
        set { maximumExperience = value; }
    }


    public float CriticalHitChance
    {
        get { return criticalHitChance; }
        set { criticalHitChance = value; }
    }

    public int PlayerLevel
    {
        get { return playerLevel; }
        set { playerLevel = value; }
    }

    public float ExperienceGainModifier
    {
        get { return experienceGainModifier; }
        set { experienceGainModifier = value; }
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
        maxhits = 100;
        damage = 10;

        maximumExperience = 100;
        playerLevel = 1;
        criticalHitChance = 0;
        experienceGainModifier = 1;

    inventory = new Inventory(this);
    }

    public void Refresh()
    {
        RefreshEquipment(GameObject.Find("ArmorSlot"), armor);
        RefreshEquipment(GameObject.Find("WeaponSlot"), weapon);
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
