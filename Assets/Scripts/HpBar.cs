using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
    public GameObject HpBarPreFab;
    public Image HpBarBackground; //Private?
    public Image HpBarFilled; //Private?


    // Start is called before the first frame update
    void Start()
    {
        HpBarBackground = Instantiate(HpBarPreFab, GameObject.Find("TheCanvas").transform).GetComponent<Image>();
        HpBarFilled = new List<Image>(HpBarBackground.GetComponentsInChildren<Image>()).Find(img => img != HpBarBackground);
    }

    // Update is called once per frame
    void Update()
    {
        HpBarBackground.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, -0.45f, 0));
    }

    public void DestroyBar()
    {
        Destroy(HpBarFilled);
        Destroy(HpBarBackground);
        Destroy(gameObject);
    }
}
