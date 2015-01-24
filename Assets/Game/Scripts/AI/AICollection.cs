using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICollection : BaseHealth
{
    private static AICollection instance;
    public static AICollection Instance { get { return instance; } }

    private IList<GameObject> allSpawns;
    public IList<GameObject> AllSpawns { get { return allSpawns; } }
    public int SpawnCount { get { return allSpawns.Count; } }

    void Awake()
    {
        instance = this;

        allSpawns = new List<GameObject>(AISpawner.MAX_SPAWNS);
    }
}
