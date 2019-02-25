using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Inventory
{
    #region Private Fields
    private GameManager manager;
    private GameObject ui;

    private PlayerState state;
    private List<Item> items;
    private List<GameObject> imageObjects;
    private Text inventoryText, selectedText;
    private int selected;
    private const int capacity = 20;

    //Controller variables
    private bool controller_neutral = true;
    private bool controller_up = false;
    private bool controller_down = false;
    #endregion

    public GameObject UI
    {
        get
        {
            if(ui == null)
                ui = GameObject.Find("Inventory");

            return ui;
        }
    }


    public Inventory(PlayerState ps)
    {
        state = ps;
        items = new List<Item>();

        manager = GameManager.instance;

        UI.SetActive(false);
    }

    public void Update(Player player)
    {
        if (manager.paused && !UI.activeSelf)
            return;

        if (Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.JoystickButton7) )
        {
            if (!manager.paused)
                Open();
            else
                Close();

            selected = 0;
            manager.paused = !manager.paused;
        }
        else if (Input.GetKeyUp(KeyCode.JoystickButton1) && manager.paused)
        {
            Close();

            selected = 0;
            manager.paused = !manager.paused;
        }
        else if (manager.paused && UI.activeSelf)
        {
            if (Input.GetKeyUp(KeyCode.DownArrow) || (((int)(Input.GetAxis("InventoryDpadVertical")) < 0) && !controller_down) || ((int)(Input.GetAxis("InventoryStickVertical")) > 0) && !controller_down)
            {
                controller_neutral = false;
                controller_down = true;

                if (++selected > capacity - 1)
                    selected = 0;

                RefreshText();

                offset -= 10;
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow) || (((int)(Input.GetAxis("InventoryDpadVertical")) > 0) && !controller_up) || ((int)(Input.GetAxis("InventoryStickVertical")) < 0) && !controller_up)
            {
                controller_neutral = false;
                controller_up = true;

                if (--selected < 0)
                    selected = capacity - 1;

                RefreshText();
                offset += 10;
            }
            else if (Input.GetKeyUp(KeyCode.U) || Input.GetKeyUp(KeyCode.JoystickButton0))
            {
                if (selected < items.Count)
                {
                    items[selected].Use(player);
                    RefreshText();
                }

            }
            else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.JoystickButton3))
            {
                if (selected < items.Count)
                    items[selected].Remove();
            }

            else if ((int)(Input.GetAxis("InventoryDpadVertical")) == 0)
            {
                controller_neutral = true;
                controller_up = false;
                controller_down = false;
            }

            UpdatePositions();
        }
    }

    public bool AddItem(Item item)
    {
        if (items.Count >= capacity)
            return false;

        items.Add(item);
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            int i = items.IndexOf(item);
            items.Remove(item);

            //Removed item cannot be equipped anymore
            if (state.Armor == item)
                state.Armor = null;
            else if (state.Weapon == item)
                state.Weapon = null;

            GameObject go = imageObjects[i];
            imageObjects.Remove(go);
            GameObject.Destroy(go);

            //Adds an empty block at the last slot to replace the old one
            AddBlock();
            //Refreshes the text for selected item
            RefreshText();
            return true;
        }

        return false;
    }

    private void RefreshText()
    {
        if (selected < items.Count)
        {
            Item item = items[selected];

            string name = item.ToString();

            if (item == state.Armor || item == state.Weapon)
                name += " (E)";

            selectedText.text = name;
        }
        else
        {
            selectedText.text = "Empty";
        }
    }

    private DateTime lastUpdate;
    private int offset;
    public void UpdatePositions()
    {
        for (int i = 0; i < imageObjects.Count; i++)
        {
            GameObject go = imageObjects[i];
            Image image = imageObjects[i].GetComponent<Image>();
            RectTransform rectTransform = image.GetComponent<RectTransform>();

            rectTransform.localPosition = GetPosition(i, offset);

            if (offset != 0 && DateTime.Now - lastUpdate > TimeSpan.FromSeconds(0.01))
            {
                lastUpdate = DateTime.Now;

                int distance = Math.Abs(offset);
                int speed = offset < 0 ? 1 : -1;

                if (distance > 10)
                    speed *= 2;


                offset += speed;
            }
        }
    }

    private Vector3 GetPosition(int i)
    {
        return GetPosition(i, 0);
    }

    private Vector3 GetPosition(int i, int offset)
    {
        int slot = i - selected;
        float points = capacity;

        double angle = (slot - (offset / 10f)) / points * 2 * Math.PI;
        angle += Math.PI / 2; //The currently selected item should be in the center of right side

        int radius = 200;

        int x = (int)(Math.Sin(angle) * radius);
        int y = (int)(Math.Cos(angle) * radius);

        return new Vector3(x, y - (250 + radius), 0);
    }

    private void Open()
    {
        UI.SetActive(true);
        imageObjects = new List<GameObject>();

        for (int i = 0; i < capacity; i++)
            AddBlock();

        inventoryText = SpriteManager.CreateText(UI.transform, 22, new Vector3(70, -200, 0), false);
        inventoryText.text = "INVENTORY " + items.Count + "/" + capacity;

        Vector3 position = GetPosition(0);
        selectedText = SpriteManager.CreateText(UI.transform, 22, new Vector3(position.x + 100, position.y, 0), false);

        RefreshText();
    }

    private void AddBlock()
    {
        SpriteManager sprites = GameManager.instance.SpriteManager;

        GameObject go = new GameObject();
        go.transform.SetParent(UI.transform);
        go.AddComponent<Image>();
        imageObjects.Add(go);

        int i = imageObjects.IndexOf(go);

        Image image = go.GetComponent<Image>();
        image.sprite = sprites.GetBlockSprite();

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        rectTransform.localPosition = GetPosition(i);

        if (i < items.Count)
            SpriteManager.CreateImage(go.transform, sprites.GetSprite(items[i].ID));
        else
            image.color = Color.grey;
    }

    private void Close()
    {
        UI.SetActive(false);
        UI.transform.DestroyChildren();
    }
}
