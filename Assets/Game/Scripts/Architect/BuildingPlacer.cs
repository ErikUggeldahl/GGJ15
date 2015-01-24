using UnityEngine;
using System.Collections;

public class BuildingPlacer : MonoBehaviour
{
    public enum BuildingType
    {
        Wall,
        Tower
    }

    [SerializeField]
    GameObject wallPreviewObj;

    [SerializeField]
    GameObject wallObj;

    [SerializeField]
    GameObject towerPreviewObj;

    [SerializeField]
    GameObject towerObj;

    [SerializeField]
    Camera camera;

    private Transform wallPreview;
    private Transform towerPreview;
    private Transform currentPreview;
    private GameObject currentToBuild;

    private int groundLayer;

    private BuildingType currentBuildingType = BuildingType.Wall;
    private bool isBuilding = false;

    void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");

        wallPreview = ((GameObject)Instantiate(wallPreviewObj)).transform;
        wallPreview.gameObject.SetActive(false);

        towerPreview = ((GameObject)Instantiate(towerPreviewObj)).transform;
        towerPreview.gameObject.SetActive(false);
    }

    public void StartBuildingWall()
    {
        StartBuilding(BuildingType.Wall);
    }

    public void StartBuildingTower()
    {
        StartBuilding(BuildingType.Tower);
    }

    public void StartBuilding(BuildingType buildingType)
    {
        currentBuildingType = buildingType;
        isBuilding = true;

        UpdatePreview(buildingType);
    }

    private void StopBuilding()
    {
        isBuilding = false;
        currentPreview.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isBuilding)
            return;

        RaycastHit hitInfo;
        var mouseRay = camera.ScreenPointToRay(Input.mousePosition);

        var isHit = Physics.Raycast(mouseRay, out hitInfo, float.PositiveInfinity, 1 << groundLayer);

        if (!isHit)
            return;

        var gridPos = GameGrid.Instance.WorldToGridPosition(hitInfo.point);
        var gridWorldPos = GameGrid.Instance.GridToWorldSpace(gridPos);

        currentPreview.position = gridWorldPos;

        if (Input.GetMouseButtonDown(0))
        {
            var gridSquare = GameGrid.Instance.GetGridSquare(gridPos);

            if (gridSquare.ResidingObject == null)
            {
                var wallGO = (GameObject)Instantiate(currentToBuild, gridWorldPos, Quaternion.identity);
                gridSquare.ResidingObject = wallGO;

                StopBuilding();
            }
        }
    }

    void UpdatePreview(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.Wall:
                currentPreview = wallPreview;
                currentToBuild = wallObj;
                break;
            case BuildingType.Tower:
                currentPreview = towerPreview;
                currentToBuild = towerObj;
                break;
        }

        currentPreview.gameObject.SetActive(true);
    }
}
