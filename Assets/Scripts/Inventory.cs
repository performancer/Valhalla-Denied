using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Inventory
{
    private GameManager manager;
    private GameObject ui;
    private int index;

    public Inventory()
    {
        manager = GameManager.instance;

        this.ui = GameObject.Find("Inventory");
        ui.SetActive(false);
        index = 0;
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
                if (++index > manager.playerItems.Count - 1)
                    index = 0;

                Refresh();
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (--index < 0)
                    index = manager.playerItems.Count - 1;

                Refresh();
            }
            else if (Input.GetKeyUp(KeyCode.U))
            {
                //TODO: use item
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                if (index < manager.playerItems.Count)
                {
                    manager.playerItems.Remove(manager.playerItems[index]);

                    if (--index < 0)
                        index = 0;
                    
                    Refresh();
                }
            }
        }
    }

    private void Refresh()
    {
        Close();
        Open();
    }

    private void Open()
    {
        ui.SetActive(true);

        Text inventoryText = GameObject.Find("InventoryText").GetComponent<Text>();
        inventoryText.text = "Inventory (" + manager.playerItems.Count + ")";

        Sprite sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/PressStart2P-Regular.ttf");

        int width = 300;
        int height = 50;

        for (int i = 0; i < manager.playerItems.Count; i++)
        {
            int x = index == i ? 0 : -25;
            int y = -(60 + i * 50);

            GameObject imageGO = new GameObject();
            imageGO.transform.SetParent(ui.transform);
            imageGO.AddComponent<Image>();

            Image image = imageGO.GetComponent<Image>();
            image.type = Image.Type.Sliced;
            image.sprite = sprite;

            SetBounds(image.GetComponent<RectTransform>(), x, y, width, height);

            GameObject textGO = new GameObject();
            textGO.transform.SetParent(ui.transform);
            textGO.AddComponent<Text>();

            Text text = textGO.GetComponent<Text>();
            text.font = font;
            text.text = manager.playerItems[i].ToString();
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;

            SetBounds(text.GetComponent<RectTransform>(), x, y, width, height);
        }
    }

    private void SetBounds(RectTransform rectTransform, int x, int y, int width, int height)
    {
        rectTransform.localPosition = new Vector3(x, y, 0);
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    private void Close()
    {
        ui.SetActive(false);

        int childs = ui.transform.childCount;

        for (int i = childs - 1; i > 0; i--)
        {
            GameObject.Destroy(ui.transform.GetChild(i).gameObject);
        }
    }
}
