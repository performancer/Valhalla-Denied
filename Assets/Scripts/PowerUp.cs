using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private GameObject pickupEffect;
    private float powerupDuration = 10.0f;

    private bool isPowerup; //set isPoison from prefab
    private bool isPoweredUp = false;
    private Color color;

    Player player;
    PlayerState state;

    public bool IsPoweredup
    {
        get { return isPoweredUp; }
        set
        {
            isPoweredUp = value;

            SpriteRenderer renderer = player.GetComponent<SpriteRenderer>();

            if (isPoweredUp)
            {
                color = renderer.color;
                renderer.color = new Color(1f, 0f, 0f, 1f);
            }
            else if (player.IsPoisoned)
            {
                renderer.color = color;
            }
            else
            {
                renderer.color = player.OriginalColor;
            }
        }
    }


    private void Start()
    {
        player = FindObjectOfType<Player>();
        state = GameManager.instance.PlayerState;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Pickup());
        }
    }

    IEnumerator Pickup()
    {
       //Player color change
        IsPoweredup = true;


        //Remove graphics and collider
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        //Stat change
        player.CritBuff = 50;

        while (isPoweredUp == true)
        {

            yield return new WaitForSeconds(powerupDuration);

            if (GameManager.instance.paused)
                continue;
            IsPoweredup = false;

            player.CritBuff = 0;
        }

        Destroy(gameObject);
    }
}




