using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private int id;
    private string name;

    public int ID { get { return id; } }

    public Item(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public virtual void Use(MovingObject from)
    {
    }

    public virtual void Remove()
    {
        Inventory inventory = GameManager.instance.PlayerState.Inventory;

        inventory.RemoveItem(this);
    }

    public override string ToString()
    {
        return name;
    } 
}
