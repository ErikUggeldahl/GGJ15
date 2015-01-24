using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TriggerType
{
	activateAgro = 0,
	loseAgro
}

public class DetectPlayers : MonoBehaviour {
	
	public TriggerType triggerType;
	// Use this for initialization
	List<Transform> listOfPlayers = new List<Transform>();
	Transform foundTarget = null;

	// Only used for agro Sphere
	void OnTriggerEnter(Collider other)
	{
		if (triggerType == TriggerType.activateAgro && other.tag == "Player") 
		{
			if (listOfPlayers.Count == 0)
				foundTarget = other.transform;

			listOfPlayers.Add(other.transform);
			Debug.Log("Player Detected");
		}
	}

	// Only used for boredAgro Sphere
	void OnTriggerExit(Collider other)
	{
		if (triggerType == TriggerType.loseAgro && other.tag == "Player") 
		{
			// If the currently found target is leaving, find a new one
			if (other.transform.GetInstanceID() == foundTarget.GetInstanceID())
				foundTarget = NearestTransformFromSelf(listOfPlayers);

			listOfPlayers.Remove(other.transform);
			Debug.Log("Player Removed");
		}
	}

	Transform NearestTransformFromSelf(List<Transform> locations)
	{
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
		return nearest;
	}
}
