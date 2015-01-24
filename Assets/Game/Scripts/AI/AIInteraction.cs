using UnityEngine;
using System.Collections;

public class AIInteraction : MonoBehaviour {

	public AIMovement movement;
	public float meleeAttackRange;
	public float attackDelay;
	bool canAttack = false;
	// Use this for initialization
	void Start () 
	{
	
	}

	public void Attack(BaseHealth target)
	{
		target.TakeDamage(1);
		StartCoroutine(delayAttack(target));
	}

	public IEnumerator delayAttack(BaseHealth target)
	{
		if (canAttack) 
		{
			yield return new WaitForSeconds(attackDelay);
			Attack(target);

			// Check if attacked target is dead
			if (!target.IsAlive)
			{
				GetComponent<TargetAssigner>().PlayerLost(target.transform);
				yield return null;
			}
		}
		else if (target.IsAlive)
			movement.Target = target.transform;

	}

	void Update()
	{
		if (AIMovement.XZdistanceBetweenTwoVec3 (transform.position, movement.Target.position) < meleeAttackRange)
			canAttack = true;
		else
			canAttack = false;

		//Debug.Log ("attack is: " + canAttack);
	}
}
