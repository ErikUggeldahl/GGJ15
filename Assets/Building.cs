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

    private int currentWood = 0;
    public int CurrentWood { get { return currentWood; } }

    private int currentStone = 0;
    public int CurrentStone { get { return currentStone; } }
	
    void Update()
    {
        if (CurrentBuildingState == BuildingState.UnderConstruction)
        {
            if(currentWood >= WoodRequired && currentStone >= StoneRequired)
            {
                ConstructBuilding();
            }
        }
    }

    private void ConstructBuilding()
    {
        CurrentBuildingState = BuildingState.Finished;

        // TODO: Shader swap, possible animations.
    }

	public bool AddResource(HarvestableResource aResource)
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
}
