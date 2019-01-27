using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageNumbers : MonoBehaviour
{

    public float textSpeed;
    public int damageNumber;

    public Text displayNumber;
    public float timeToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        displayNumber.text = "" + damageNumber;
        transform.position = new Vector3(transform.position.x, transform.position.y + textSpeed * Time.deltaTime, transform.position.z); //teksti menee ylös textSpeedin mukaan

        timeToDestroy -= Time.deltaTime;

        if(timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
        
    }
}
