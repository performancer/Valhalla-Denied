using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private string name;

    public Item(string name)
    {
        this.name = name;
    }

    public override string ToString()
    {
        return name;
    }
}
