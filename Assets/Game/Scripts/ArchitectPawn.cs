using UnityEngine;
using System.Collections;

public class ArchitectPawn : MonoBehaviour, IHealth
{
    private int health;
    public int Health { set { health = value; } get { return health; } }

    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage(int aDamage)
    {
        if (!IsDead)
        {
            Health -= aDamage;

            if (Health <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        isDead = true;
    }

    public void Respawn()
    {
        isDead = false;
    }
}
