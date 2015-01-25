using UnityEngine;
using System.Collections;

public class WallDetecter : MonoBehaviour {

	public AIMovement aiMovement;

	void OnTriggerEnter(Collider other)
	{
		Building building = other.GetComponent<Building>();
		if (building != null && building.CurrentBuildingState == Building.BuildingState.Finished) 
		{
			aiMovement.StructureToDestroy = building;
		}
	}
}
