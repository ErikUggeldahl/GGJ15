﻿using UnityEngine;
using System.Collections;

public class AIHealth : BaseHealth
{
    void Start()
    {
        SetStartingHP();

        AICollection.Instance.AllSpawns.Add(this.gameObject);
    }

    protected override void Die()
    {
        base.Die();

        AICollection.Instance.AllSpawns.Remove(this.gameObject);
        Destroy(gameObject);
    }
}
