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
    private bool open;

    private PlayerState state;
    private List<Item> items;
    private List<GameObject> imageObjects;
    private Text inventoryText, selectedText;
    private int selected;
    private const int capacity = 20;

    //Controller variables
    private bool controllerUp = false;
    private bool controllerDown = false;
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
        if (manager.paused && !open)
            return;

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.JoystickButton7))
        {
            if (!manager.paused)
                Open();
            else
                Close();

            manager.paused = !manager.paused;
        }
        else if ((Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace) || Input.GetKeyUp(KeyCode.JoystickButton1)) && open)
        {
            Close();
            manager.paused = false;
        }
        else if (open)
        {
            if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S) || (((int)(Input.GetAxis("InventoryDpadVertical")) < 0) && !controllerDown) || ((int)(Input.GetAxis("InventoryStickVertical")) > 0) && !controllerDown)
            {
                controllerDown = true;

                if (++selected > capacity - 1)
                    selected = 0;

                RefreshText();

                offset -= 10;
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W) || (((int)(Input.GetAxis("InventoryDpadVertical")) > 0) && !controllerUp) || ((int)(Input.GetAxis("InventoryStickVertical")) < 0) && !controllerUp)
            {
                controllerUp = true;

                if (--selected < 0)
                    selected = capacity - 1;

                RefreshText();
                offset += 10;
            }
            else if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.JoystickButton0))
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
                controllerUp = false;
                controllerDown = false;
            }

            UpdatePositions();
        }
    }


    private DateTime lastMessageTime;
    public bool AddItem(MovingObject mo, Item item)
    {
        if (items.Count >= capacity)
        {
            if (DateTime.Now - lastMessageTime > TimeSpan.FromSeconds(2))
            {
                lastMessageTime = DateTime.Now;
                mo.CreateFloatingText("Inventory Full", Color.grey);
            }
            return false;
        }

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
        open = true;
        UI.SetActive(true);
        imageObjects = new List<GameObject>();

        selected = 0;

        for (int i = 0; i < capacity; i++)
            AddBlock();

        inventoryText = SpriteManager.CreateText(UI.transform, 22, new Vector3(70, -200, 0), false);
        inventoryText.text = "INVENTORY " + items.Count + "/" + capacity;

        string uitext;

        if (DetectXboxController() == true)
        {
            uitext = "Press A to Use\nPress Y to Drop\nPress B to close";
        }
        else
        {
            uitext = "Press E to Use\nPress D to Drop\nPress Backspace to close";

        }
        SpriteManager.CreateText(UI.transform, 14, new Vector3(300, -250, 0), false).text = uitext;

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
        open = false;
        UI.SetActive(false);
        UI.transform.DestroyChildren();

        selected = 0;
    }

    private bool DetectXboxController()
    {
        string[] names = Input.GetJoystickNames();

        bool isXboxController = false;

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == "Controller (Xbox One For Windows)")
            {
                isXboxController = true;
            }
            else
            {
                isXboxController = false;
            }
        }

        if (isXboxController == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
