using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.


public class BoardManager : MonoBehaviour
{
    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.


        //Assignment constructor.
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    private int columns;                                            //Number of columns in our game board.
    private int rows;                                               //Number of rows in our game board.
    private Count wallCount;                                        //Lower and upper limit for our random number of walls per level.
    private Count foodCount = new Count(1, 5);                       //Lower and upper limit for our random number of food items per level.
    private int scrollCount;
    public GameObject exit;                                         //Prefab to spawn for exit.
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] foodTiles;                                  //Array of food prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
    public GameObject[] obstacleWallTiles;
    public GameObject[] scrollTiles;
    public GameObject[] swordTiles;
    public GameObject[] armorTiles;
    public GameObject[] legendarySwordTiles;
    public GameObject[] legendaryArmorTiles;
    public GameObject[] powerupTiles;

    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private List<Vector3> gridPositions = new List<Vector3>();  //A list of possible locations to place tiles.

    public bool IsBossRoom { get; set; }
    public bool IsTutorial { get; set; }

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 0; x < columns; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 0; y < rows; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    private List<Vector3> doorPositions;

    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup(bool isBoss)
    {
        IsBossRoom = isBoss;

        int mode = Random.Range(0, 4);

        int horizontalWall = Random.Range(2, rows - 2);
        int verticalWall = Random.Range(2, columns - 2);

        switch (mode)
        {
            case 0: horizontalWall = -1; verticalWall = -1; break;
            case 1: horizontalWall = -1; break;
            case 2: verticalWall = -1; break;
        }

        int horizontalDoor = Random.Range(0, verticalWall - 1);
        int horizontalDoorAlt = Random.Range(verticalWall + 1, columns - 1);

        if (verticalWall == -1)
        {
            horizontalDoor = Random.Range(0, columns - 1);
            horizontalDoorAlt = -1;
        }

        int verticalDoor = Random.Range(0, horizontalWall - 1);
        int verticalDoorAlt = Random.Range(horizontalWall + 1, rows - 1);

        if (horizontalWall == -1)
        {
            verticalDoor = Random.Range(0, rows - 1);
            verticalDoorAlt = -1;
        }

        doorPositions = new List<Vector3>();

        if (!isBoss)
            gridPositions.RemoveAll(item => item.y == horizontalWall || item.x == verticalWall);

        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for (int x = -1; x < columns + 1; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = -1; y < rows + 1; y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                else if (!isBoss && (x == verticalWall || y == horizontalWall))
                {
                    if (y != verticalDoor && y != verticalDoorAlt && x != horizontalDoor && x != horizontalDoorAlt)
                        //toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                        toInstantiate = obstacleWallTiles[Random.Range(0, obstacleWallTiles.Length)];
                    else
                        doorPositions.Add(new Vector3(x, y));
                }

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }
    }


    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);

        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }

    Vector3 CenterPosition()
    {
        for (int radius = 0; radius < 5; radius++)
        {
            for (int x = rows / 2 - radius; x <= rows / 2 + radius; x++)
            {
                for (int y = columns / 2 - radius; y <= columns / 2 + radius; y++)
                {
                    if (gridPositions.Contains(new Vector3(x, y)))
                        return new Vector3(x, y);
                }
            }
        }

        return RandomPosition();
    }

    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene(int level, bool isBoss)
    {
        columns = Random.Range(8, 10 + level);
        rows = Random.Range(8, 10 + level);

        int area = (columns - 1) * (rows - 1);
        wallCount = new Count((int)(area * 0.1f), (int)(area * 0.5f));

        //Reset our list of gridpositions.
        InitialiseList();

        //Creates the outer walls and floor.
        BoardSetup(isBoss);

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);

        if (isBoss)
        {
            int enemyChoice;
            if (GameManager.instance.CheckIfTutorial() == true)
            {
                enemyChoice = 0;
            }
            else
            {
                enemyChoice = Random.Range(0, enemyTiles.Length);
            }

            GameObject boss = Instantiate(enemyTiles[enemyChoice], CenterPosition(), Quaternion.identity);

            SpriteRenderer renderer = boss.GetComponent<SpriteRenderer>();
            renderer.color = new Color(0.8f, 0.45f, 0.45f, Random.Range(0, 1) == 1 ? 1 : 0.8f);
            Transform tran = boss.GetComponent<Transform>();
            tran.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        }
        else
        {
            //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

            int scrollCount;
            int scrollChance = Random.Range(1, 4);

            GameManager manager = FindObjectOfType<GameManager>();

            if (scrollChance == 1 && manager.GetLevel() > 2)
                scrollCount = 1;
            else
                scrollCount = 0;

            //Instantiate scroll tiles, at randomized positions
            LayoutObjectAtRandom(scrollTiles, scrollCount, scrollCount);

            int enemyCount;
            if (level == 1)
            {
                enemyCount = 0;
            }
            else if (level == 2)
            {
                enemyCount = 1;
            }
            else
            {
                enemyCount = (int)(Mathf.Log(level, 2f) * (area / 64f));
            }

            //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

            int swordCount;
            int swordChance = Random.Range(1, 10);

            if ((swordChance == 1 && manager.CheckIfTutorial() == false) || manager.GetLevel() == 1)
                swordCount = 1;
            else
                swordCount = 0;

            LayoutObjectAtRandom(swordTiles, swordCount, swordCount);

            int armorCount;
            int armorChance = Random.Range(1, 10);

            if ((armorChance == 1 && manager.CheckIfTutorial() == false) || manager.GetLevel() == 1)
                armorCount = 1;
            else
                armorCount = 0;

            LayoutObjectAtRandom(armorTiles, armorCount, armorCount);

            int powerupCount;
            int powerupChance;
            
                powerupChance = Random.Range(1, 3);

            if (powerupChance == 1 && manager.CheckIfTutorial() == false || manager.GetLevel() == 2)
                powerupCount = 1;
            else
                powerupCount = 0;

            LayoutObjectAtRandom(powerupTiles, powerupCount, powerupCount);

            //Instantiate the exit tile in random position
            CreateRandomExit();
        }
    }

    public void CreateRandomExit()
    {
        Vector3 position;

        //not very reliable but it should do it's work unless there's just no space for the door anywhere, which is almost impossible with current settings
        //this has to go since project is due tomorrow :)
        while (true)
        {
            bool retry = false;
            position = RandomPosition();

            for (int i = 0; i < doorPositions.Count; i++)
            {
                if (Vector3.Distance(position, doorPositions[i]) <= 1)
                {
                    retry = true;
                    break;
                }
            }

            if (!retry)
                break;
        }

        Instantiate(exit, position, Quaternion.identity);
    }

    public void CreateSword()
    {
        if (IsBossRoom)
        {
            GameManager manager = FindObjectOfType<GameManager>();

            int legendaryChance = Random.Range(1, 100);

            if (manager.GetLevel() >= 10 && legendaryChance <= 5 + manager.GetLevel())
            {
                Instantiate(legendarySwordTiles[0], RandomPosition(), Quaternion.identity);
            }
            else
            {
                Instantiate(swordTiles[0], RandomPosition(), Quaternion.identity);
            }
        }
        else
        {
            Instantiate(swordTiles[0], RandomPosition(), Quaternion.identity);
        }

    }

    public void CreateArmor()
    {
        if (IsBossRoom)
        {
            GameManager manager = FindObjectOfType<GameManager>();
            int legendaryChance = Random.Range(1, 100);
            if (manager.GetLevel() >= 10 && legendaryChance <= 5 + manager.GetLevel())
            {
                Instantiate(legendaryArmorTiles[0], RandomPosition(), Quaternion.identity);
            }
            else
            {
                Instantiate(armorTiles[0], RandomPosition(), Quaternion.identity);
            }
        }
        else
        {
            Instantiate(armorTiles[0], RandomPosition(), Quaternion.identity);
        }
    }
}

