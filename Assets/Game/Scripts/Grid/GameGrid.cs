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

    void Awake()
    {
        instance = this;
        CreateGrid();
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

    public Vector3 GridToWorldSpace(GridPosition aPosition)
    {
        return new Vector3(aPosition.X - (gridSizeX / 2), 0, aPosition.Y - (gridSizeY / 2));
    }

    public GridPosition WorldToGridPosition(Vector3 aPosition)
    {
        return new GridPosition(Mathf.RoundToInt(aPosition.x) + (gridSizeX/2), Mathf.RoundToInt(aPosition.z) + (gridSizeY/2));
    }
}
