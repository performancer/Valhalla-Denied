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
    private Text selectedText;
    private int selected;

    private Sprite blockSprite;
    private Sprite[] itemSprites;
    #endregion

    public Inventory(PlayerState ps)
    {
        state = ps;
        items = new List<Item>();

        manager = GameManager.instance;

        ui = GameObject.Find("Inventory");
        ui.SetActive(false);

        UnityEngine.Object[] spriteSheet = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Scavengers_SpriteSheet.png");

        itemSprites = new Sprite[2];
        itemSprites[0] = (Sprite)spriteSheet[19];
        itemSprites[1] = (Sprite)spriteSheet[20];

        blockSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/woodenmenu.png");
    }

    public void Update(Player player)
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            if (!manager.paused)
                Open();
            else
                Close();

            selected = 0;
            manager.paused = !manager.paused;
        }
        else if (manager.paused)
        {
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (++selected > Player.MaxInventory - 1)
                    selected = 0;

                RefreshText();

                offset -= 10;
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (--selected < 0)
                    selected = Player.MaxInventory - 1;

                RefreshText();

                offset += 10;
            }
            else if (Input.GetKeyUp(KeyCode.U))
            {
                if (selected < items.Count)
                    items[selected].Use(player);
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                if (selected < items.Count)
                    items[selected].Remove();
            }

            UpdatePositions();
        }
    }

    public bool AddItem(Item item)
    {
        if (items.Count >= 20)
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
        float points = Player.MaxInventory;

        double angle = ( slot - (offset / 10f))/ points * 2 * Math.PI;
        angle += Math.PI / 2; //The currently selected item should be in the center of right side

        int radius = 200;

        int x = (int)(Math.Sin(angle) * radius);
        int y = (int)(Math.Cos(angle) * radius);

        return new Vector3(x - 50, y - (300 + radius), 0);
    }

    private void Open()
    {
        if (ui == null)
            ui = GameObject.Find("Inventory");

        ui.SetActive(true);
        imageObjects = new List<GameObject>();

        Text inventoryText = GameObject.Find("InventoryText").GetComponent<Text>();
        inventoryText.text = "Inventory " + items.Count + "/" + Player.MaxInventory;

        Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/PressStart2P-Regular.ttf");

        for (int i = 0; i < Player.MaxInventory; i++)
            AddBlock();

        //This is the text for the selected item
        GameObject textGO = new GameObject();
        textGO.transform.SetParent(ui.transform);
        textGO.AddComponent<Text>();

        Text text = textGO.GetComponent<Text>();
        text.font = font;
        text.fontSize = 22;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        selectedText = text;

        Vector3 position = GetPosition(0);
        RectTransform textTransform = text.GetComponent<RectTransform>();
        textTransform.localPosition = new Vector3(position.x + 140, position.y + 30, 0);

        RefreshText();
    }

    private void AddBlock()
    {
        GameObject go = new GameObject();
        go.transform.SetParent(ui.transform);
        go.AddComponent<Image>();
        imageObjects.Add(go);

        int i = imageObjects.IndexOf(go);

        Image image = go.GetComponent<Image>();
        image.sprite = blockSprite;

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        rectTransform.localPosition = GetPosition(i);

        if (i < items.Count)
        {
            Item item = items[i];

            GameObject child = new GameObject();
            child.transform.SetParent(go.transform);
            child.AddComponent<Image>();

            Image thumbnail = child.GetComponent<Image>();
            thumbnail.sprite = itemSprites[item.ID];

            RectTransform thumbform = thumbnail.GetComponent<RectTransform>();
            thumbform.localPosition = new Vector3(50, 50, 0);
            thumbform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
        else
        {
            image.color = Color.grey;
        }
    }

    private void Close()
    {
        ui.SetActive(false);

        int childs = ui.transform.childCount;

        for (int i = childs - 1; i > 0; i--)
            GameObject.Destroy(ui.transform.GetChild(i).gameObject);
    }
}
