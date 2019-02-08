using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingNumbers : MonoBehaviour
{
    public GameObject FloatingNumberPreFab;
    public Text FloatingNumberText;

    public float TextSpeed;
    public int  numberToDisplay;
    public float timeToDestroy;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    
    public void setColor(int red, int green, int blue)
    {
        FloatingNumberText.color = new Color(red, green, blue);
    }
    

    // Update is called once per frame
    void Update()
    {


        FloatingNumberText.text = "" + numberToDisplay;

        transform.position = new Vector3(transform.position.x, transform.position.y + TextSpeed * Time.deltaTime, transform.position.z); //teksti menee ylös textSpeedin mukaan

        // FloatingNumberText.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(transform.position.x, transform.position.y + TextSpeed * Time.deltaTime, transform.position.z));

        timeToDestroy -= Time.deltaTime;

        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }


    }
}
