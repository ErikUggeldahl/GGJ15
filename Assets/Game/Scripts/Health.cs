using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public int startingHealth;

	float currentHealth;
	float CurrentHealth
	{
		get { return currentHealth; }
		set { currentHealth = value; }
	}

	delegate void DeathDel();
}
