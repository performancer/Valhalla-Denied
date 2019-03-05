using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    PlayerState state;
    Player player;

    private bool isPowerup; //set isPoison from prefab
    private float powerupInterval = 2;
    private float powerupCount = 3;
    private bool isPoweredup = false;
    private Color color;

    public bool IsPoweredup
    {
        get { return isPoweredup; }
        set
        {
            isPoweredup = value;

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            if (isPoweredup)
            {
                color = renderer.color;
                renderer.color = new Color(0f, 1f, 0f, 1f);
            }
            else
            {
                renderer.color = color;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameManager.instance.PlayerState;
        player = FindObjectOfType<Player>();
    }

    public void QuadDamage()
    {
        Debug.Log("QUAD DAMAGE!");
    }

    public void Speed()
    {
        
    }

    public void Protection()
    {

    }

    public IEnumerator Powerup(int pDmg)
    {
        float powerupCounter = 0;

        IsPoweredup = true;

        while (powerupCounter < powerupCount && IsPoweredup)
        {
            yield return new WaitForSeconds(powerupInterval);

            state.Hits += pDmg;
            //player.CreateFloatingText(pDmg.ToString(), Color.magenta);

            //player.UpdateHpBar();

            powerupCounter++;
        }

        yield return new WaitForSeconds(powerupInterval);

        IsPoweredup = false;
    }
}
