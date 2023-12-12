using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridSystem : MonoBehaviour
{
    private int levelSeed;
    [SerializeField]
    private int gridSize = 10;
    [SerializeField]
    private int maxShipSize = 5;
    [SerializeField]
    private int sonarRadius = 2;
    [SerializeField]
    private int ShipQuantity;

    [SerializeField]
    private Sprite ShipSize1;
    [SerializeField]
    private Sprite ShipSize2;
    [SerializeField]
    private Sprite ShipSize3;
    [SerializeField]
    private Sprite ShipSize4;

    [SerializeField]
    private Transform GridAnchor;

    [SerializeField]
    private GameObject tile;

    private int[] headersX;
    private int[] sunkenHeadersX;
    private int[] totalHeadersX;
    [SerializeField]
    private GameObject headersXPrefab;
    private GameObject[] textHeadersX;

    private int[] headersY;
    private int[] sunkenHeadersY;
    private int[] totalHeadersY;

    [SerializeField]
    private GameObject headersYPrefab;
    private GameObject[] textHeadersY;

    private int shotsFired = 0;
    private int totalShipPieces;
    [SerializeField]
    private GameObject shotsFiredText;
    [SerializeField]
    private Text[] shipsInfoText;

    [SerializeField]
    private GameObject WinScreen;
    [SerializeField]
    private Text WinText;

    private GameObject[,] grid;
    private int[] ShipCounts;
    private List<Ship> shipList = new List<Ship>();
    private bool missileMode = false;

    System.Random seededRandom;

    [SerializeField]
    private AdsSystem adSystem;
    // Start is called before the first frame update
    void Start()
    {
        levelSeed = PlayerPrefs.GetInt("currentLevelSeed");
        seededRandom = new System.Random(levelSeed);
        InitializeBoard();
    }


    public void InitializeBoard()
    {
        WinScreen.SetActive(false);
        headersX = new int[gridSize];
        sunkenHeadersX = new int[gridSize];
        totalHeadersX = new int[gridSize];
        textHeadersX = new GameObject[gridSize];
        headersY = new int[gridSize];
        sunkenHeadersY = new int[gridSize];
        totalHeadersY = new int[gridSize];
        textHeadersY = new GameObject[gridSize];
        grid = new GameObject[gridSize, gridSize];
        ShipCounts = new int[maxShipSize];
        for (int i = 0; i < gridSize; i++)
        {
            Vector3 pos = new Vector3(GridAnchor.position.x + (i * 80), GridAnchor.position.y + 80, 0);
            Vector3 pos1 = new Vector3(GridAnchor.position.x - 80, GridAnchor.position.y - (i * 80), 0);
            textHeadersX[i] = Instantiate(headersXPrefab, pos, Quaternion.identity, GridAnchor);
            textHeadersY[i] = Instantiate(headersYPrefab, pos1, Quaternion.Euler(0, 0, 90), GridAnchor);
        }
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 pos = new Vector3(GridAnchor.position.x + (i * 80), GridAnchor.position.y - (j * 80), 0);
                grid[i, j] = Instantiate(tile, pos, Quaternion.identity, GridAnchor);
                grid[i, j].GetComponent<Tile>().grid = this;
                grid[i, j].GetComponent<Tile>().coordX = i;
                grid[i, j].GetComponent<Tile>().coordY = j;
            }
        }

        SpawnShips();
        UpdateBorders();
    }

    private void SpawnShips()
    {
        int spawnedShips = 0;
        while (spawnedShips < ShipQuantity)
        {
            int size = seededRandom.Next(2, maxShipSize+1);
            bool canSpawn = false;
            while (canSpawn == false)
            {
                List<int> sides = new List<int> { 0, 1, 2, 3 };

                int x = Random.Range(0, gridSize);
                int y = Random.Range(0, gridSize);
                Debug.Log("spawning size " + size + " at x:" + x + " y:" + y);
                Ship tempShip = new Ship(size, size - 1);
                Sprite tempImage;
                switch (size)
                {
                    case 2:
                        tempImage = ShipSize1;
                        break;
                    case 3:
                        tempImage = ShipSize2;
                        break;
                    case 4:
                        tempImage = ShipSize3;
                        break;
                    case 5:
                        tempImage = ShipSize4;
                        break;
                    default:
                        tempImage = ShipSize1;
                        break;
                }

                if (x > gridSize - size)
                {
                    sides.Remove(1);
                }
                else if (x < size - 1)
                {
                    sides.Remove(3);
                }
                if (y > gridSize - size)
                {
                    sides.Remove(2);
                }
                else if (y < size - 1)
                {
                    sides.Remove(0);
                }

                while (sides.Count > 0)
                {
                    int side = sides[Random.Range(0, sides.Count)];
                    sides.Remove(side);
                    Debug.Log("selected side: " + side);
                    canSpawn = true;

                    switch (side)
                    {
                        case 0:
                            for (int i = 0; i < size; i++)
                            {
                                if (grid[x, y - i].GetComponent<Tile>().Occupied())
                                {
                                    canSpawn = false;
                                }
                            }
                            if (canSpawn)
                            {

                                for (int i = 0; i < size; i++)
                                {
                                    grid[x, y - i].GetComponent<Tile>().SetContent(tempShip);
                                    grid[x, y - i].transform.GetChild(0).GetComponent<Image>().sprite = tempImage;


                                }
                            }

                            break;
                        case 1:
                            for (int i = 0; i < size; i++)
                            {
                                if (grid[x + i, y].GetComponent<Tile>().Occupied())
                                {
                                    canSpawn = false;
                                }
                            }
                            if (canSpawn)
                            {
                                for (int i = 0; i < size; i++)
                                {
                                    grid[x + i, y].GetComponent<Tile>().SetContent(tempShip);
                                    grid[x + i, y].transform.GetChild(0).GetComponent<Image>().sprite = tempImage;

                                }
                            }
                            break;
                        case 2:
                            for (int i = 0; i < size; i++)
                            {
                                if (grid[x, y + i].GetComponent<Tile>().Occupied())
                                {
                                    canSpawn = false;
                                }
                            }
                            if (canSpawn)
                            {
                                for (int i = 0; i < size; i++)
                                {
                                    grid[x, y + i].GetComponent<Tile>().SetContent(tempShip);
                                    grid[x, y + i].transform.GetChild(0).GetComponent<Image>().sprite = tempImage;

                                }
                            }

                            break;
                        case 3:
                            for (int i = 0; i < size; i++)
                            {
                                if (grid[x - i, y].GetComponent<Tile>().Occupied())
                                {
                                    canSpawn = false;
                                }
                            }
                            if (canSpawn)
                            {
                                for (int i = 0; i < size; i++)
                                {
                                    grid[x - i, y].GetComponent<Tile>().SetContent(tempShip);
                                    grid[x - i, y].transform.GetChild(0).GetComponent<Image>().sprite = tempImage;
                                }
                            }
                            break;
                    }
                    if (canSpawn)
                    {
                        ShipCounts[size - 2]++;
                        spawnedShips++;

                        break;
                    }
                    
                }
            }
        }
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Tile tile = grid[i, j].GetComponent<Tile>();
                if (tile.GetContent() != null )
                {
                    int value = tile.GetContent().getValue();
                   
                    totalHeadersX[i] += value;
                    totalHeadersY[j] += value;
                }
            }
        }

        int textCounter = 0;
        foreach(Text textShip in shipsInfoText)
        {
            totalShipPieces += ShipCounts[textCounter] * (textCounter + 2);
            textShip.text = ": " + ShipCounts[textCounter];
            textCounter++;
        }
    }

    public void changeActive(bool missile)
    {
        if (missile)
        {
            missileMode = true;
        }
        else
        {
            missileMode = false;
        }
    }

    public void click(Tile tile)
    {
        shotsFired++;
        shotsFiredText.GetComponent<TextMeshProUGUI>().text = "Shots: "+shotsFired;
        if (missileMode)
        {
            SendMissile(tile);
        }
        else
        {
            SendMissile(tile);
        }


    }

    private void SendMissile(Tile tile)
    {
        bool hit = tile.Attack();

        if (hit)
        {

            sunkenHeadersX[tile.coordX] += tile.GetContent().getValue();
            sunkenHeadersY[tile.coordY] += tile.GetContent().getValue();

            if (sunkenHeadersX[tile.coordX] == totalHeadersX[tile.coordX])
            {
                for (int i = 0; i < gridSize; i++)
                {
                    grid[tile.coordX, i].GetComponent<Tile>().Attack();
                }
            }

            if (sunkenHeadersY[tile.coordY] == totalHeadersY[tile.coordY])
            {
                for (int i = 0; i < gridSize; i++)
                {
                    grid[i, tile.coordY].GetComponent<Tile>().Attack();
                }
            }

            if (tile.GetContent().HasBeenSunk())
            {
                ShipCounts[tile.GetContent().getSize() - 2]--;
                int textCounter = 0;
                foreach (Text textShip in shipsInfoText)
                {
                    textShip.text = ": " + ShipCounts[textCounter];
                    textCounter++;
                }

            }
        }
        UpdateBorders();
        CheckIfWon();
    }

    private void SendProbe(Tile tile)
    {
        int lowerBoundX = tile.coordX - sonarRadius < 0 ? 0 : tile.coordX - sonarRadius;
        int upperBoundX = tile.coordX + sonarRadius > gridSize-1 ? gridSize - 1 : tile.coordX + sonarRadius;
        int lowerBoundY = tile.coordY - sonarRadius < 0 ? 0 : tile.coordY - sonarRadius;
        int upperBoundY = tile.coordY + sonarRadius > gridSize - 1 ? gridSize - 1 : tile.coordY + sonarRadius;

        for (int i = lowerBoundX; i <= upperBoundX; i++)
        {
            for (int j = lowerBoundY; j <= upperBoundY; j++)
            {
                grid[i, j].GetComponent<Tile>().Discover();
            }
        }
    }

    public void GiveHint()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Debug.Log(this.grid[i,j]);
                Tile tile = grid[i, j].GetComponent<Tile>();
                if (tile.GetContent() != null && !tile.IsDiscovered())
                {
                    
                    SendMissile(tile);
                    goto end;
                }
            }
        }
    end:;
    }

    private void UpdateBorders()
    {
        
        for (int i = 0; i < gridSize; i++)
        {
            headersX[i] = 0;
            headersY[i] = 0;
        }

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Tile tile = grid[i, j].GetComponent<Tile>();
                //&& tile.IsDiscovered()
                if (tile.GetContent() != null )
                {
                    int value = tile.GetContent().getValue();
                //Debug Line
                //grid[i, j].transform.GetChild(0).GetComponent<Text>().text = value.ToString();
                    //
                    headersX[i] += value;
                    headersY[j] += value;
                }
            }
        }

        for (int i = 0; i < gridSize; i++)
        {
            textHeadersX[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = headersX[i].ToString();
            textHeadersY[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = headersY[i].ToString();
        }
    }

    private void CheckIfWon()
    {
        bool hasWon = true;
       
        foreach (int counter in ShipCounts)
        {
            if (counter > 0)
            {
                hasWon = false;
                

                break;
            }
        }
        if (hasWon)
        {
            
            switch ((int)Mathf.Floor(levelSeed / 1000))
            {
                case 1:
                    if(levelSeed%1000 > PlayerPrefs.GetInt("LastEasyLevelCompleted", 0))
                    {
                        PlayerPrefs.SetInt("LastEasyLevelCompleted", levelSeed % 1000);
                    }
                    
                   
                    break;
                case 2:
                    if (levelSeed % 1000 > PlayerPrefs.GetInt("LastNormalLevelCompleted", 0))
                    {
                        PlayerPrefs.SetInt("LastNormalLevelCompleted", levelSeed % 1000);
                    }
                   
                    break;
                case 3:
                    if (levelSeed % 1000 > PlayerPrefs.GetInt("LastHardLevelCompleted", 0))
                    {
                        PlayerPrefs.SetInt("LastHardLevelCompleted", levelSeed % 1000);
                    }
                    break;
            }
            StartCoroutine(SendBackToMenu());
        }
    }

    IEnumerator SendBackToMenu()
    {
        Debug.Log("Returning to Menu");
        yield return new WaitForSeconds(1);
        if (levelSeed % 3 == 0)
        {
            adSystem.ShowInterstitialAd();
        }
        
        ShowWonScreen();
    }

    private void ShowWonScreen()
    {
        WinScreen.SetActive(true);

        Debug.Log(Mathf.Ceil(totalShipPieces * 1.1f) + " with shots "+shotsFired);
        if (shotsFired <= Mathf.Ceil(totalShipPieces * 1.1f))
        {
            WinText.text = "3 stars";
        }
        else {
            if(shotsFired <= Mathf.Ceil(totalShipPieces * 1.3f))
            {
                WinText.text = "2 stars";
            }
            else
            {
                WinText.text = "1 star";
            }
        }

    }

    
}
