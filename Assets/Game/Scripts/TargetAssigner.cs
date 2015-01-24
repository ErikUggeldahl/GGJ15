using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum AIActionStates
{
	movingTowardsKing = 0,
	movingTowardsTarget,
	attacking
}

public class TargetAssigner : MonoBehaviour {

	public AIMovement movement;
	public Transform TestTarget;

	public SphereCollider agroSphere;
	public SphereCollider boredSphere;
	public Transform foundTarget = null;
	public List<Transform> listOfPlayers = new List<Transform>();

	void Awake()
	{
		movement.Target = TestTarget;

	}
	// Use this for initialization
	void Start ()
	{
		movement.OnMovementFinish += OnMovementDone;
	}

	void OnMovementDone(Transform target)
	{

	}

	public void PlayerDetected(Transform player)
	{
		if (listOfPlayers.Count == 0 || player.tag == "King") 
		{
			Debug.Log("New Target is: " + player);
			foundTarget = player;
		}
		
		listOfPlayers.Add(player);
		Debug.Log("Player Detected");
	}

	public void PlayerLost(Transform player)
	{
		if (listOfPlayers.Find (x => x.GetInstanceID () == player.GetInstanceID ()) != null)
			listOfPlayers.Remove (player);
		else 
		{
			Debug.Log ("Player left bored Sphere without entering agro first");
			return;
		}

		// If the currently found target is leaving, find a new one
		if (player.GetInstanceID() == foundTarget.GetInstanceID() && foundTarget.tag != "King")
			foundTarget = NearestTransformFromSelf(listOfPlayers);

		Debug.Log("Player Removed");
	}

	public Transform NearestTransformFromSelf(List<Transform> locations)
	{
		// If there is only one more player in range, get him
		if (locations.Count == 1)
			return locations [0];
		else if (locations.Count == 0)
			return null;
		
		Transform nearest = locations[0];
		float nearestDistance = Vector3.Distance(locations[0].position, transform.position);
		for (int i = 1; i < locations.Count; i++)
		{
			if(Vector3.Distance(locations[i].position, transform.position) < nearestDistance)
			{
				nearest = locations[i];
				nearestDistance = Vector3.Distance(locations[i].position, transform.position);
			}
		}
		Debug.Log ("Nearest Player is: " + nearest.name);
		return nearest;
	}
}
