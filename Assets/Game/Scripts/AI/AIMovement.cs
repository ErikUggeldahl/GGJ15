using UnityEngine;
using System.Collections;

public class AIMovement : MonoBehaviour 
{

	const float AI_TARGET_TRESHOLD = 1.2f;


	Transform target;
	public Transform Target
	{
		get { return target; }
		set 
		{ 
			target = value; 
			StopCoroutine("Moving");
			StartCoroutine(Moving());
		}
	}
	
	float speedMultiplier;
	public float SpeedMultiplier
	{
		set 
		{
			speedMultiplier = value; 
			movingSpeed = defaultMovingSpeed * speedMultiplier;
		}
	}

	public float defaultMovingSpeed;
	public float maxSpeed;
	float defaultdrag;
	public bool canMove = true;

	float movingSpeed;

	public float slowDownTime;
	public float slowDownDragRate;

	public delegate void MovementFinishDel(Transform target);
	public event MovementFinishDel OnMovementFinish;

	// Use this for initialization
	void Start () 
	{
		movingSpeed = defaultMovingSpeed;
		defaultdrag = rigidbody.drag;
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
		yield return new WaitForSeconds(timeToSlowDown);
		rigidbody.drag = defaultdrag;
	}

	public static float XZdistanceBetweenTwoVec3(Vector3 location1, Vector3 location2)
	{
		return Vector3.Distance (Vector3.Scale (location1, Vector3.forward + Vector3.right), Vector3.Scale (location2, Vector3.forward + Vector3.right));
	}
}
