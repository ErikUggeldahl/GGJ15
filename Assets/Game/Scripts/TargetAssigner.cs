using UnityEngine;
using System.Collections;

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
}
