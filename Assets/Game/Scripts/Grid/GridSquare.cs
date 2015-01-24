using UnityEngine;
using System.Collections;

public class GridSquare : MonoBehaviour 
{
    public GridPosition Position;
    public GameObject ResidingObject = null;

    public void Init(int aX, int aY)
    {
        Position = new GridPosition(aX, aY);
        transform.position = GameGrid.Instance.GridToWorldSpace(Position);
        transform.rotation = Quaternion.LookRotation(Vector3.down);

        gameObject.AddComponent<MeshFilter>().mesh = GameGrid.Instance.gridQuad;
        gameObject.AddComponent<MeshRenderer>().material = GameGrid.Instance.gridMaterial;
        gameObject.AddComponent<BoxCollider>();
    }
}
