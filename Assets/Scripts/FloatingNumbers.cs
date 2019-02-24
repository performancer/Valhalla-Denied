using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingNumbers : MonoBehaviour
{
    public GameObject FloatingNumberPreFab;
    public Text FloatingNumberText;

    public float TextSpeed;
    public string  textToDisplay;
    public float timeToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        FloatingNumberPreFab = (GameObject)Resources.Load("FloatingNumbers");
    }

    public void setColor(Color color)
    {
        FloatingNumberText.color = color;
    }
    
    // Update is called once per frame
    void Update()
    {
        FloatingNumberText.text = "" + textToDisplay;

        transform.position = new Vector3(transform.position.x, transform.position.y + TextSpeed * Time.deltaTime, transform.position.z); //teksti menee ylös textSpeedin mukaan

        timeToDestroy -= Time.deltaTime;

        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }

}
