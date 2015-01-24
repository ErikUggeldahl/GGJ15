using UnityEngine;
using System.Collections;

public class BaseHealth : MonoBehaviour 
{
	public int startingHP;

	int health;
	public int Health
	{
		get { return health; }
		set { health = value; }
	}
	
	bool isAlive = true;
	public bool IsAlive 
	{
		get{ return isAlive; }
		set{ isAlive = value; }
	}

	protected virtual void SetStartingHP ()
	{
		Health = startingHP;
	}

	protected virtual void Respawn()
	{

	}
	
	public void TakeDamage(int aDamage)
	{
		Debug.Log (gameObject.name + " Took " + aDamage + " of damage");
		Health -= aDamage;

		if (Health <= 0 && isAlive)
			Die();
	}
	
	protected virtual void Die()
	{
		Debug.Log(gameObject.name + " is dead");
		IsAlive = false;
	}


	// Use this for initialization
	void Start () 
	{

	}
}
