using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Building : MonoBehaviour
{
    public enum BuildingState
    {
        UnderConstruction,
        Finished
    }

    private static List<Building> allBuildings = new List<Building>();
    public static Building[] AllBuildings { get { return allBuildings.ToArray(); } }

    protected BuildingState currentBuildingState = BuildingState.UnderConstruction;
    public BuildingState CurrentBuildingState { get { return currentBuildingState; } }

    public Material UnderConstructionMaterial;
    public Material FinishedMaterial;

    public List<GameObject> BuildingRenderObjects = new List<GameObject>();

    public int WoodRequired;
    public int StoneRequired;

    public int currentWood = 0;
    public int CurrentWood { get { return currentWood; } }

    public int currentStone = 0;
    public int CurrentStone { get { return currentStone; } }

    public static Building GetNearestBuilding (Vector3 point)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < allBuildings.Count; i++)
        {
            float currentDistance = (allBuildings[i].transform.position - point).magnitude;
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestIndex = i;
            }
        }

        if (closestIndex >= 0)
            return allBuildings[closestIndex];
        else return null;
    }


    protected virtual void OnEnable()
    {
        allBuildings.Add(this);
    }

    protected virtual void OnDisable()
    {
        allBuildings.Remove(this);
    }

    protected virtual void CheckBuld()
    {
        if (CurrentBuildingState == BuildingState.UnderConstruction)
        {
            if (currentWood >= WoodRequired && currentStone >= StoneRequired)
            {
                ConstructBuilding();
            }
        }
    }

    public void Cancel()
    {
        if (currentBuildingState != BuildingState.UnderConstruction)
        {
            Debug.LogWarning("Attempting to cancel a completed building.");
            return;
        }

        Destroy(gameObject);
    }

    protected virtual void ConstructBuilding()
    {
        currentBuildingState = BuildingState.Finished;

        foreach (GameObject go in BuildingRenderObjects)
        {
            go.renderer.material = FinishedMaterial;
        }
    }

    public virtual bool ConsumeResource(HarvestableResource aResource)
    {
        if (currentBuildingState == BuildingState.Finished || (WoodRequired <= 0 && StoneRequired <= 0))
            return false;

        if (aResource.gameObject.GetComponent<Tree>() != null)
        {
            Destroy(aResource.gameObject);
            currentWood++;
            CheckBuld();
            return true;
        }
        else if (aResource.gameObject.GetComponent<Rock>() != null)
        {
            Destroy(aResource.gameObject);
            currentStone++;
            CheckBuld();
            return true;
        }

        return false;
    }

    protected virtual void OnTriggerEnter(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
            BuilderPawn builderPawn = aCollider.gameObject.GetComponent<BuilderPawn>();

            builderPawn.NearbyBuildings.Add(this);
        }
    }

    protected virtual void OnTriggerExit(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
            BuilderPawn builderPawn = aCollider.gameObject.GetComponent<BuilderPawn>();

            builderPawn.NearbyBuildings.Remove(this);
        }
    }
}
