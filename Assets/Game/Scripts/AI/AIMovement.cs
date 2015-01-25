using UnityEngine;
using System.Collections;

public class AIMovement : MonoBehaviour 
{
    public GameObject frozenMesh;

	const float AI_TARGET_TRESHOLD = 3.2f;
	
	Transform target;
	public Transform Target
	{
		get { return target; }
		set 
		{ 
			canMove = true;
			target = value; 
			StopCoroutine("Moving");
			StartCoroutine(Moving());
		}
	}

	Building structureToDestroy;
	public Building StructureToDestroy
	{
		get { return structureToDestroy; }
		set 
		{
			structureToDestroy = value;
			canMove = false;
			StartCoroutine(ClearWay(structureToDestroy));
		}
	}

	float defaultdrag;
	float defaultAngularDrag;

	public float defaultMaxSpeed;
	float maxSpeed;
	
	public bool canMove = true;

	public float movingSpeed;

	public float slowDownTime;
	public float slowDownDragRate;
	public float slowDownAngularDrag;

	public delegate void MovementFinishDel(Transform target);
	public event MovementFinishDel OnMovementFinish;

	// Use this for initialization
	void Start () 
	{
		maxSpeed = defaultMaxSpeed;
		defaultdrag = rigidbody.drag;
		defaultAngularDrag = rigidbody.angularDrag;
	}

	IEnumerator Moving()
	{
		while (XZdistanceBetweenTwoVec3(transform.position, target.position) > AI_TARGET_TRESHOLD)
		{
			if (target != null && canMove) 
			{
				transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
				if (rigidbody.velocity.magnitude < maxSpeed)
					rigidbody.AddForce(transform.forward * movingSpeed, ForceMode.VelocityChange );
			}
			yield return new WaitForFixedUpdate();
		}
		OnMovementFinish.Invoke(target);
		StartCoroutine(SlowDown(slowDownTime, slowDownDragRate));
	}

	IEnumerator SlowDown(float timeToSlowDown, float dragRate)
	{
		rigidbody.drag = dragRate;
		rigidbody.angularDrag = slowDownAngularDrag;
		yield return new WaitForSeconds(timeToSlowDown);
		rigidbody.drag = defaultdrag;
		rigidbody.angularDrag = defaultAngularDrag;
	}

	IEnumerator ClearWay(Building building)
	{
		BaseHealth wallHealth = building.GetComponent<BaseHealth>();
		AIInteraction aiInteract = GetComponent<AIInteraction>();
		while (wallHealth.IsAlive) 
		{
			wallHealth.TakeDamage(aiInteract.damage, transform);
			yield return new WaitForSeconds(aiInteract.attackDelay);
		}
		canMove = true;
	}

    public void StartStun(float time, float slowdownPercentage)
    {
        StartCoroutine(Stun(time, slowdownPercentage));
        frozenMesh.SetActive(true);
    }

	private IEnumerator Stun(float time, float slowdownPercentage)
	{
		if (slowdownPercentage == 0)
			rigidbody.velocity = Vector3.zero;

		maxSpeed = slowdownPercentage * maxSpeed;
		yield return new WaitForSeconds(time);
		maxSpeed = defaultMaxSpeed;
        frozenMesh.SetActive(false);
	}

	public static float XZdistanceBetweenTwoVec3(Vector3 location1, Vector3 location2)
	{
		return Vector3.Distance (Vector3.Scale (location1, Vector3.forward + Vector3.right), Vector3.Scale (location2, Vector3.forward + Vector3.right));
	}
}
