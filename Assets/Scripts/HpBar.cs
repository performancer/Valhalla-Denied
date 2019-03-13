using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
    public GameObject HpBarPreFab;
    public Image HpBarBackground;
    public Image HpBarFilled;

    BoardManager board;


    // Start is called before the first frame update
    void Start()
    {
        board = GameManager.instance.BoardManager;

            HpBarBackground = Instantiate(HpBarPreFab, GameObject.Find("EnemyHpBarCanvas").transform).GetComponent<Image>();
            HpBarFilled = new List<Image>(HpBarBackground.GetComponentsInChildren<Image>()).Find(img => img != HpBarBackground);

        if (board.IsBossRoom)
        {
            HpBarBackground.rectTransform.localScale = new Vector3(1.5f, 1.0f, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (board.IsBossRoom)
        {
            HpBarBackground.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, -0.675f, 0));
        }
        else
        {
            HpBarBackground.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, -0.45f, 0));
        }
    }

    public void DestroyBar()
    {
        Destroy(HpBarFilled);
        Destroy(HpBarBackground);
        Destroy(gameObject);
    }

}


