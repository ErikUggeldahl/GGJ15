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

    public static GridPosition operator +(GridPosition x, GridPosition y)
    {
        return new GridPosition(x.Y + y.Y, x.X + y.X);
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

    public float clearingRadius = 20.0f;

    public float gridScale = 1;

    public int gridSizeX = 100;
    public int gridSizeY = 100;

    GridSquare[,] gridSquares;

    public Mesh gridQuad;
    public Material gridMaterial;

    public Tree TreePrefab;
    public int TreeClusters = 20;
    public int MaxTreeClusterCount = 20;
    public int MinTreeClusterCount = 10;
    public float TreeClusterRadius = 10.0f;

    public Rock RockPrefab;
    public int RockCount = 5;

    void Awake()
    {
        instance = this;
        CreateGrid();
        PopulateResources();
        Shader.SetGlobalFloat("_ClearingRadius", clearingRadius);
    }

    public GridSquare GetGridSquare(GridPosition aPosition)
    {
        if (aPosition.X < gridSizeX && aPosition.Y < gridSizeY && aPosition.X >= 0 && aPosition.Y >= 0)
            return gridSquares[aPosition.X, aPosition.Y];
        else
            return null;
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

    bool IsInClearing(GridPosition aPosition)
    {
        Vector3 worldSpace = GridToWorldSpace(aPosition);
        Vector3 center = GridToWorldSpace(new GridPosition(gridSizeX/2,gridSizeY/2));
        float distance = (worldSpace - center).magnitude;
        return distance < clearingRadius;
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
            while (IsInClearing(newPosition) || GetGridSquare(newPosition) == null || GetGridSquare(newPosition).ResidingObject != null);

            GetGridSquare(newPosition).ResidingObject = (Instantiate(RockPrefab, GridToWorldSpace(newPosition), Quaternion.identity) as Rock).gameObject;
        }
    }

    void PopulateTrees()
    {
        for (int i = 0; i < TreeClusters; i++)
        {
            GridPosition clusterPosition;
            do
            {
                clusterPosition = new GridPosition(Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
            }
            while (IsInClearing(clusterPosition) || GetGridSquare(clusterPosition).ResidingObject != null);

            int ClusterSize = Random.Range(MinTreeClusterCount,MaxTreeClusterCount);

            for (int c = 0; c < ClusterSize; c++)
            {
                GridPosition spawnPosition;
                int halfRadius = (int)TreeClusterRadius / 2;

                do
                {
                    spawnPosition = new GridPosition(Random.Range(-halfRadius, halfRadius), Random.Range(-halfRadius, halfRadius));
                    spawnPosition += clusterPosition;
                }
                while (IsInClearing(spawnPosition) || GetGridSquare(spawnPosition) == null || GetGridSquare(spawnPosition).ResidingObject != null);

                Tree newTree = Instantiate(TreePrefab, GridToWorldSpace(spawnPosition), Quaternion.identity) as Tree;
                GetGridSquare(spawnPosition).ResidingObject = newTree.gameObject;
            }
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
