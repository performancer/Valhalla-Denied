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

    public virtual bool Use(Player player)
    {
        return false;
    }

    public override string ToString()
    {
        return name;
    }
}
