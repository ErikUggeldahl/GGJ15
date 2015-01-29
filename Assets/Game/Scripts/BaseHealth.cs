using UnityEngine;
using System.Collections;

public class BaseHealth : MonoBehaviour 
{
	public int startingHP = 10;

	public int health;
	public int Health { get { return health; } set {  health = value; } }
	
	protected bool isAlive = true;
	public bool IsAlive 
	{
		get{ return isAlive; }
		set
		{
            //Debug.Log("isAlive being set");
			isAlive = value;
		}
	}

	protected virtual void SetStartingHP ()
	{
		Health = startingHP;
	}

	public virtual void Respawn()
	{
        SetStartingHP();
        isAlive = true;
	}
	
	public virtual void TakeDamage(int aDamage, Transform aggressor)
	{
        if (isAlive)
        {
            //Debug.Log(gameObject.name + " Took " + aDamage + " of damage");
            Health -= aDamage;

            if (Health <= 0)
                Die();
        }
	}
	
	protected virtual void Die()
	{
		//Debug.Log(gameObject.name + " is dead");
        isAlive = false;
	}
}
