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
	public AIInteraction interact;

	public SphereCollider agroSphere;
	public SphereCollider boredSphere;
	internal List<Transform> listOfPlayers = new List<Transform>();

	// Use this for initialization
	void Start ()
	{
		StartCoroutine(WaitForArchitectToSpawn());
		movement.OnMovementFinish += OnMovementDone;
	}

	IEnumerator WaitForArchitectToSpawn()
	{
		yield return new WaitForSeconds(1f);
		movement.Target = Game.Instance.ArchitectPawn.transform;
	}

	void OnMovementDone(Transform target)
	{
		//Debug.Log ("Done Moving");
		BaseHealth targetHealth = target.GetComponent<BaseHealth> ();
		if (targetHealth != null && targetHealth.IsAlive)
			interact.Attack(targetHealth);
		//else
			//Debug.LogWarning (target.name +" Does not have health but AI is trying to attack it");
	}

	public void PlayerDetected(Transform player)
	{
		if (listOfPlayers.Count == 0 || player.tag == "Architect") 
		{
			//Debug.Log("New Target is: " + player);
			movement.Target = player;
		}
		
		listOfPlayers.Add(player);
		//Debug.Log("Player Detected");
	}

	public void PlayerLost(Transform player)
	{
		List<Transform> allPlayers;
		allPlayers = listOfPlayers.FindAll (x => x.GetInstanceID () == player.GetInstanceID ());
		if(allPlayers.Count != 0)
			listOfPlayers.RemoveAll(x => x.GetInstanceID () == player.GetInstanceID ());
		else 
		{
			//Debug.Log (player.name + " left bored Sphere without entering agro first");
			return;
		}

		// If the currently found target is leaving, find a new one
		if (player.GetInstanceID() == movement.Target.GetInstanceID() && movement.Target.tag != "Architect")
			movement.Target = NearestTransformFromSelf(listOfPlayers);

		//Debug.Log("Player Removed");
	}

	public Transform NearestTransformFromSelf(List<Transform> locations)
	{
		// If there is only one more player in range, get him
		if (locations.Count == 1)
			return locations [0];
		else if (locations.Count == 0)
			return Game.Instance.ArchitectPawn.transform;
		
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
		//Debug.Log ("Nearest Player is: " + nearest.name);
		return nearest;
	}
}
