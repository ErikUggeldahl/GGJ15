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
    StunAbility stun;

    [SerializeField]
    GameObject wallPreviewObj;

    [SerializeField]
    GameObject wallObj;

    [SerializeField]
    GameObject towerPreviewObj;

    [SerializeField]
    GameObject towerObj;

    private Camera sceneCamera;

    private Transform wallPreview;
    private Transform towerPreview;
    private Transform currentPreview;
    private GameObject currentToBuild;

    private int groundLayerMask;
    private int buildingLayerMask;

    private bool isBuilding = false;

    void Start()
    {
        sceneCamera = Camera.main;

        wallPreview = ((GameObject)Instantiate(wallPreviewObj)).transform;
        wallPreview.gameObject.SetActive(false);

        towerPreview = ((GameObject)Instantiate(towerPreviewObj)).transform;
        towerPreview.gameObject.SetActive(false);

        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
        buildingLayerMask = 1 << LayerMask.NameToLayer("Building");
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
        stun.DisableStunning();

        isBuilding = true;

        UpdatePreview(buildingType);
    }

    private void StopBuilding()
    {
        stun.EnableStunningDelayed();

        isBuilding = false;
        currentPreview.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isBuilding)
            CastBuild();
        else
            CastCancel();
    }

    private void CastBuild()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StopBuilding();
            return;
        }

        RaycastHit hitInfo;
        var mouseRay = sceneCamera.ScreenPointToRay(Input.mousePosition);

        var isHit = Physics.Raycast(mouseRay, out hitInfo, float.PositiveInfinity, groundLayerMask);

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

    private void CastCancel()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        RaycastHit hitInfo;
        var mouseRay = sceneCamera.ScreenPointToRay(Input.mousePosition);

        var isHit = Physics.Raycast(mouseRay, out hitInfo, float.PositiveInfinity, buildingLayerMask);

        if (!isHit)
            return;

        Building building = hitInfo.transform.GetComponent<Building>();
        if (building == null)
        {
            Debug.LogWarning("No building script on object to cancel.");
            return;
        }

        if (building.CurrentBuildingState != Building.BuildingState.UnderConstruction)
            return;

        building.Cancel();
    }

    private void UpdatePreview(BuildingType buildingType)
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
