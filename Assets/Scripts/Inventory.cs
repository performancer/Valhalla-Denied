using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Inventory
{
    private Player player;
    private GameManager manager;
    private GameObject ui;

    private List<GameObject> imageObjects;
    private Text selectedItemName;
    private int index;

    private Sprite[] sprites;

    public Inventory(Player player)
    {
        this.player = player;

        manager = GameManager.instance;

        this.ui = GameObject.Find("Inventory");
        ui.SetActive(false);

        UnityEngine.Object[] spriteSheet = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Scavengers_SpriteSheet.png");

        sprites = new Sprite[2];
        sprites[0] = (Sprite)spriteSheet[19];
        sprites[1] = (Sprite)spriteSheet[20];
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            if (!manager.paused)
                Open();
            else
                Close();

            index = 0;
            manager.paused = !manager.paused;
        }
        else if (manager.paused)
        {
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (++index > Player.MaxInventory - 1)
                    index = 0;

                Refresh();

                offset = -10;
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (--index < 0)
                    index = Player.MaxInventory - 1;

                Refresh();

                offset = 10;
            }
            else if (Input.GetKeyUp(KeyCode.U))
            {
                if (index < player.Items.Count)
                {
                    Item item = player.Items[index];

                    if (item.Use(player))
                        RemoveItem(index);
                    else
                        Refresh();
                }
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                if (index < player.Items.Count)
                    RemoveItem(index);
            }


            UpdatePositions();
        }
    }

    private void RemoveItem(int i)
    {
        Item item = player.Items[i];

        if (item == player.Armor)
            player.Armor = null;
        else if (item == player.Weapon)
            player.Weapon = null;

        player.Items.Remove(item);

        GameObject go = imageObjects[i];
        imageObjects.Remove(go);
        GameObject.Destroy(go);

        if (--index < 0)
            index = 0;

        Refresh();
    }

    private void Refresh()
    {
        UpdateName();
    }

    private void UpdateName()
    {
        if (index < player.Items.Count)
        {
            Item item = player.Items[index];

            string name = item.ToString();

            if (item == player.Armor || item == player.Weapon)
                name += " (E)";

            selectedItemName.text = name;
        }
        else
        {
            selectedItemName.text = "Empty";
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
                if (offset < 0)
                    offset++;
                else
                    offset--;
            }
        }
    }

    private Vector3 GetPosition(int i)
    {
        return GetPosition(i, 0);
    }

    private Vector3 GetPosition(int i, int offset)
    {
        int slot = i - index;
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
        ui.SetActive(true);
        imageObjects = new List<GameObject>();

        Text inventoryText = GameObject.Find("InventoryText").GetComponent<Text>();
        inventoryText.text = "Inventory " + player.Items.Count + "/" + Player.MaxInventory;

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/woodenmenu.png");
        Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/PressStart2P-Regular.ttf");

        for (int i = 0; i < Player.MaxInventory; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(ui.transform);
            go.AddComponent<Image>();
            imageObjects.Add(go);

            Image image = go.GetComponent<Image>();
            image.sprite = sprite;

            RectTransform rectTransform = image.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            rectTransform.localPosition = GetPosition(i);

            if (i < player.Items.Count)
            {
                Item item = player.Items[i];

                GameObject child = new GameObject();
                child.transform.SetParent(go.transform);
                child.AddComponent<Image>();

                Image thumbnail = child.GetComponent<Image>();
                thumbnail.sprite = sprites[item.ID];

                RectTransform thumbform = thumbnail.GetComponent<RectTransform>();
                thumbform.localPosition = new Vector3(50, 50, 0);
                thumbform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }
            else
            {
                image.color = Color.grey;
            }
        }

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
        selectedItemName = text;

        Vector3 position = GetPosition(0);
        RectTransform textTransform = text.GetComponent<RectTransform>();
        textTransform.localPosition = new Vector3(position.x + 140, position.y + 30, 0);

        UpdateName();
    }

    private void Close()
    {
        ui.SetActive(false);

        int childs = ui.transform.childCount;

        for (int i = childs - 1; i > 0; i--)
            GameObject.Destroy(ui.transform.GetChild(i).gameObject);
    }
}
