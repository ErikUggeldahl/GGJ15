﻿using UnityEngine;
using System.Collections;

public class AIHealth : BaseHealth
{
    public GameObject GhostPrefab = null;

    void Start()
    {
        SetStartingHP();

        AICollection.Instance.AllSpawns.Add(this.gameObject);
    }

    protected override void Die()
    {
        base.Die();

        AICollection.Instance.AllSpawns.Remove(this.gameObject);

        Instantiate(GhostPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }

	public override void TakeDamage (int aDamage, Transform aggressor)
	{
		base.TakeDamage (aDamage, aggressor);
		//GetComponent<AIMovement>().Target = aggressor;
	}
}
