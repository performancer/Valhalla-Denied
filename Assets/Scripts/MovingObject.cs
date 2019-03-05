using System;
using UnityEngine;
using System.Collections;

//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
public abstract class MovingObject : MonoBehaviour
{
    public bool isPoison; //set isPoison from prefab
    public float poisonInterval = 2;
    public float poisonCount = 3;
    public int poisonDamage; //set poisonDamage from prefab

    #region Private Fields
    private BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
    private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
    private LayerMask blockingLayer;

    //private readonly float moveTime = 0.1f; //0.1f default //Time it will take object to move, in seconds.
    public float moveTime = 0.1f; //Time it will take object to move, in seconds. //Set in PreFab editor
    public float moveDelayVar; //Set in PreFab editor

    private float inverseMoveTime;          //Used to make movement more efficient.

    private DateTime lastMove;
    private TimeSpan moveDelay;             //Delay between movements

    private int maxHits, hits;
    private int damage;

    private bool isCritical;

    private GameObject FloatingNumberPreFab;

    private bool isPoisoned = false;
    private Color color;

    #endregion

    #region Properties
    public DateTime LastMove
    {
        get { return lastMove; }
        set { lastMove = value; }
    }

    public TimeSpan MoveDelay
    {
        get { return moveDelay; }
        set { moveDelay = value; }
    }

    public virtual int MaxHits
    {
        get { return maxHits; }
        set { maxHits = value; }
    }

    public virtual int Hits
    {
        get { return hits; }
        set { hits = value; }
    }

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public int PoisonDamage
    {
        get { return poisonDamage; }
        set { poisonDamage = value; }
    }

    public bool IsPoisoned
    {
        get { return isPoisoned; }
        set
        {
            isPoisoned = value;

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            if (isPoisoned)
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
    #endregion

    protected virtual void Start()
    {
        FloatingNumberPreFab = (GameObject)Resources.Load("FloatingNumbers");

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        blockingLayer = LayerMask.GetMask("BlockingLayer");

        //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
        inverseMoveTime = 1f / moveTime;

        moveDelay = TimeSpan.FromSeconds(moveDelayVar); //0.8 default
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        boxCollider.enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(start, end, blockingLayer);

        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }


    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            boxCollider.offset = end - newPosition;
            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPosition);

            OnMovement();

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }

    protected virtual void OnMovement()
    {
    }

    //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
    //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        LastMove = DateTime.Now;

        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        //Check if nothing was hit by linecast
        if (hit.transform == null)
            return;

        //Get a component reference to the component of type T attached to the object that was hit
        T hitComponent = hit.transform.GetComponent<T>();

        //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }


    //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
    //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
    //OnCantMove will be overriden by functions in the inheriting classes.
    protected abstract void OnCantMove<T>(T component)
        where T : Component;

    public virtual void LoseHits(int dmg, bool isCrit, bool isPoison, int poisondmg)
    {
        Hits -= dmg;
    }

    public IEnumerator Poison(int pDmg)
    {
        float poisonCounter = 0;

        IsPoisoned = true;

        while (poisonCounter < poisonCount && IsPoisoned)
        {
            yield return new WaitForSeconds(poisonInterval);

            Hits -= pDmg;
            CreateFloatingText(pDmg.ToString(), Color.magenta);

            UpdateHpBar();

            poisonCounter++;
        }

        yield return new WaitForSeconds(poisonInterval);

        IsPoisoned = false;
    }

    public void CreateFloatingText(string text, Color color)
    {
        var clone = Instantiate(FloatingNumberPreFab, transform.position, Quaternion.Euler(Vector3.zero));
        clone.transform.SetParent(GameObject.Find("FloatingTextCanvas").transform);

        clone.GetComponent<FloatingNumbers>().textToDisplay = text;
        clone.GetComponent<FloatingNumbers>().SetColor(color);
    }

    public abstract void UpdateHpBar();
    

}

