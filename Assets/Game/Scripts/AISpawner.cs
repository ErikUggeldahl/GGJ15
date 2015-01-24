using UnityEngine;
using System.Collections;

public class AISpawner : MonoBehaviour
{
    [SerializeField]
    GameObject SpawnObj;

    private const float HALF_PI = Mathf.PI / 2f;
    private const float WORLD_SIZE = 100f;

    private const float SPAWN_TIMER = 10f;
    private float numberToSpawn = 1f;
    private const float SPAWN_ACCELERATION_FACTOR = 1.1f;
    public const int MAX_SPAWNS = 300;

    private Transform spawnParent;

    void Start()
    {
        spawnParent = new GameObject("Spawn Parent").transform;
        

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            var spawnCount = Mathf.Min(MAX_SPAWNS - AICollection.Instance.SpawnCount, numberToSpawn);

            for (int i = 0; i < spawnCount; i++)
            {
                var spawn = (GameObject)Instantiate(SpawnObj, RandomEdgeWorldPosition(), Quaternion.identity);
                spawn.transform.SetParent(spawnParent);
            }

            numberToSpawn *= SPAWN_ACCELERATION_FACTOR;

            yield return new WaitForSeconds(SPAWN_TIMER);
        }
    }

    void Update()
    {
    }

    private Vector3 RandomEdgeWorldPosition()
    {
        return EdgePositionToWorld(PercentToEdgePosition(Random.value));
    }

    private Vector2 PercentToEdgePosition(float inPercent)
    {
        var percent = Mathf.Clamp01(inPercent);
        var result = Vector2.zero;

        if (percent < 0.25f)
        {
            var theta = Mathf.Lerp(0f, HALF_PI, percent * 4f);
            result.x = Mathf.Sin(theta);
        }
        else if (percent < 0.5f)
        {
            result.x = 1f;

            var theta = Mathf.Lerp(0f, HALF_PI, (percent - 0.25f) * 4f);
            result.y = Mathf.Sin(theta);
        }
        else if (percent < 0.75f)
        {
            var theta = Mathf.Lerp(HALF_PI, 0f, (percent - 0.5f) * 4f);
            result.x = Mathf.Sin(theta);

            result.y = 1f;
        }
        else
        {
            var theta = Mathf.Lerp(HALF_PI, 0f, (percent - 0.75f) * 4f);
            result.y = Mathf.Sin(theta);
        }

        return result;
    }

    private Vector3 EdgePositionToWorld(Vector2 edgePos)
    {
        return new Vector3((edgePos.x - 0.5f) * WORLD_SIZE, 0f, (edgePos.y - 0.5f) * WORLD_SIZE);
    }
}
