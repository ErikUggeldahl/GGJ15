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

    private BuildingState currentBuildingState = BuildingState.UnderConstruction;
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

    protected virtual void Update()
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

    public virtual bool AddResource(HarvestableResource aResource)
    {
        if (aResource.gameObject.GetComponent<Tree>() != null)
        {
            if (WoodRequired > 0)
            {
                Destroy(aResource.gameObject);
                currentWood++;

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (aResource.gameObject.GetComponent<Rock>() != null)
        {
            if (StoneRequired > 0)
            {
                Destroy(aResource.gameObject);
                currentStone++;

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
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
