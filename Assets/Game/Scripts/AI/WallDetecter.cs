using UnityEngine;
using System.Collections;

public class WallDetecter : MonoBehaviour {

	public AIMovement aiMovement;

	void OnTriggerEnter(Collider other)
	{
		Wall wall = other.GetComponent<Wall>();
		if (wall != null) 
		{
			aiMovement.WallToDestroy = wall;
		}
	}
}
