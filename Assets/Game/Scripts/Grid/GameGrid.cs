using UnityEngine;
using System.Collections;

public struct GridPosition
{
    public int X;
    public int Y;

    public GridPosition(int aX, int aY)
    {
        X = aX;
        Y = aY;
    }
}

public class GameGrid : MonoBehaviour 
{
    static GameGrid instance;
    public static GameGrid Instance
    {
        get
        {
            return instance;
        }
    }

    public float gridScale = 1;

    public int gridSizeX = 100;
    public int gridSizeY = 100;

    GridSquare[,] gridSquares;

    public Mesh gridQuad;
    public Material gridMaterial;

    public Tree TreePrefab;
    public int TreeCount = 50;

    public Rock RockPrefab;
    public int RockCount = 5;

    void Awake()
    {
        instance = this;
        CreateGrid();
        PopulateResources();
    }

    public GridSquare GetGridSquare(GridPosition aPosition)
    {
        return gridSquares[aPosition.X, aPosition.Y];
    }

    void CreateGrid()
    {
        gridSquares = new GridSquare[gridSizeX,gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {                
                GameObject newSquareObj = new GameObject("GridSquare x" + x + " y" + y );
                newSquareObj.transform.SetParent(this.transform);

                GridSquare newSquare = newSquareObj.AddComponent<GridSquare>();
                newSquare.Init(x, y);
                gridSquares[x, y] = newSquare;
            }
        }
    }

    void PopulateResources()
    {
        PopulateRocks();
        PopulateTrees();
    }

    void PopulateRocks()
    {
        for (int i = 0; i < RockCount; i++)
        {
            GridPosition newPosition;
            do
            {
                newPosition = new GridPosition(Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
            }
            while (GetGridSquare(newPosition).ResidingObject != null);

            GetGridSquare(newPosition).ResidingObject = (Instantiate(RockPrefab, GridToWorldSpace(newPosition), Quaternion.identity) as Rock).gameObject;
        }
    }

    void PopulateTrees()
    {
        for (int i = 0; i < TreeCount; i++)
        {
            GridPosition newPosition;
            do
            {
                newPosition = new GridPosition(Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
            }
            while (GetGridSquare(newPosition).ResidingObject != null);

            GetGridSquare(newPosition).ResidingObject = (Instantiate(TreePrefab, GridToWorldSpace(newPosition), Quaternion.identity) as Tree).gameObject;
        }
    }

    public Vector3 GridToWorldSpace(GridPosition aPosition)
    {
        return new Vector3(aPosition.X - (gridSizeX / 2), 0, aPosition.Y - (gridSizeY / 2));
    }

    public GridPosition WorldToGridPosition(Vector3 aPosition)
    {
        return new GridPosition(Mathf.RoundToInt(aPosition.x) + (gridSizeX/2), Mathf.RoundToInt(aPosition.z) + (gridSizeY/2));
    }
}
