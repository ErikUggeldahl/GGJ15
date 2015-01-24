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
	public TargetAssigner targetAssign;

	// Only used for agro Sphere
	void OnTriggerEnter(Collider other)
	{
		if ((other.tag == "Builder" || other.tag == "King") && triggerType == TriggerType.activateAgro) 
			targetAssign.PlayerDetected(other.transform);
	}

	// Only used for boredAgro Sphere
	void OnTriggerExit(Collider other)
	{
		if ((other.tag == "Builder" || other.tag == "King") && triggerType == TriggerType.loseAgro) 
			targetAssign.PlayerLost(other.transform);
	}
}
