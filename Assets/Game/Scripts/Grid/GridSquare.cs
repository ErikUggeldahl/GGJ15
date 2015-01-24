using UnityEngine;
using System.Collections;

public class GridSquare : MonoBehaviour 
{
    private static int groundLayer;
    public static int GroundLayer { set { groundLayer = value; }}

    public GridPosition Position;
    public GameObject ResidingObject = null;

    public void Init(int aX, int aY)
    {
        Position = new GridPosition(aX, aY);
        transform.position = GameGrid.Instance.GridToWorldSpace(Position);

        gameObject.layer = groundLayer;
    }
}
