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
		StopCoroutine(Moving());
	}

	IEnumerator SlowDown(float timeToSlowDown, float dragRate)
	{
		rigidbody.drag = dragRate;
		rigidbody.angularDrag = slowDownAngularDrag;
		yield return new WaitForSeconds(timeToSlowDown);
		rigidbody.drag = defaultdrag;
		rigidbody.angularDrag = defaultAngularDrag;
	}

    public void StartStun(float time, float slowdownPercentage)
    {
        StartCoroutine(Stun(time, slowdownPercentage));
    }

	private IEnumerator Stun(float time, float slowdownPercentage)
	{
		maxSpeed = slowdownPercentage * maxSpeed;
		yield return new WaitForSeconds(time);
		maxSpeed = defaultMaxSpeed;
	}

	public static float XZdistanceBetweenTwoVec3(Vector3 location1, Vector3 location2)
	{
		return Vector3.Distance (Vector3.Scale (location1, Vector3.forward + Vector3.right), Vector3.Scale (location2, Vector3.forward + Vector3.right));
	}
}
