using UnityEngine;
using System.Collections;

public class AIInteraction : MonoBehaviour {

	public AIMovement movement;
	public float meleeAttackRange;
	public float attackDelay;
	public int damage;
	bool canAttack = false;
	// Use this for initialization
	void Start () 
	{
	
	}

//	private IEnumerator DashForward()
//	{
//		Vector3 initPos = transform.position;
//		transform.Translate(transform.forward * 0.2f);
//		yield return new WaitForSeconds(0.5f);
//		transform.position = initPos;
//	}

	public void Attack(BaseHealth target)
	{
		transform.LookAt(new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z));
		//StartCoroutine(DashForward());
		target.TakeDamage(damage);
		StartCoroutine(delayAttack(target));
	}

	IEnumerator delayAttack(BaseHealth target)
	{
		if (canAttack) 
		{
			yield return new WaitForSeconds(attackDelay);
			Attack(target);

			// Check if attacked target is dead
			if (!target.IsAlive)
			{
				GetComponent<TargetAssigner>().PlayerLost(target.transform);
				StopCoroutine(delayAttack(target));
			}
		}
		else if (target.IsAlive)
			movement.Target = target.transform;
	}

	void Update()
	{
		if (movement.Target != null) 
		{
			if (AIMovement.XZdistanceBetweenTwoVec3 (transform.position, movement.Target.position) < meleeAttackRange)
				canAttack = true;
			else
				canAttack = false;
		}
	}
}
