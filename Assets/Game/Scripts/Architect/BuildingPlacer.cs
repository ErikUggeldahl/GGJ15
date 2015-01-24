using UnityEngine;
using System.Collections;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField]
    GameObject wallPreviewObj;

    [SerializeField]
    GameObject wallObj;

    [SerializeField]
    Camera camera;

    private Transform wallPreview;

    private int groundLayer;

    void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");

        wallPreview = ((GameObject)Instantiate(wallPreviewObj)).transform;
    }

    void Update()
    {
        RaycastHit hitInfo;
        var mouseRay = camera.ScreenPointToRay(Input.mousePosition);

        var isHit = Physics.Raycast(mouseRay, out hitInfo, float.PositiveInfinity, 1 << groundLayer);

        if (!isHit)
            return;

        var gridPos = GameGrid.Instance.WorldToGridPosition(hitInfo.point);
        var gridWorldPos = GameGrid.Instance.GridToWorldSpace(gridPos);

        wallPreview.position = gridWorldPos;

        if (Input.GetMouseButtonDown(0))
        {
            var gridSquare = GameGrid.Instance.GetGridSquare(gridPos);

            if (gridSquare.ResidingObject == null)
            {
                var wallGO = (GameObject)Instantiate(wallObj, gridWorldPos, Quaternion.identity);
                gridSquare.ResidingObject = wallGO;
            }
        }
    }
}
