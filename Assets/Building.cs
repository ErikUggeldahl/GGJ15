using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
    public enum BuildingState
    {
        UnderConstruction,
        Finished
    }

    public BuildingState CurrentBuildingState = BuildingState.UnderConstruction;

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
            if(currentWood >= WoodRequired && currentStone >= StoneRequired)
            {
                ConstructBuilding();
            }
        }
    }

    protected virtual void ConstructBuilding()
    {
        CurrentBuildingState = BuildingState.Finished;

        this.renderer.material.shader = Shader.Find("Custom/Illustrative");
    }

    public virtual bool AddResource(HarvestableResource aResource)
    {
        if(aResource.gameObject.GetComponent<Tree>() != null)
        {
            if(WoodRequired > 0)
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
