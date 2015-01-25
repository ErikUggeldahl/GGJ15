using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : Building
{
	public float TowerRange = 5;

    public float fireDelay = 1.0f;

    float refireCounter = 0.0f;

	public Transform TurretObject;

    public GameObject projectilePrefab;

    public Transform Target;

    float GetRange(Transform aTarget)
    {
        return (transform.position - aTarget.transform.position).magnitude;
    }

	GameObject FindTarget()
	{
        IList<GameObject> allAI = AICollection.Instance.AllSpawns;

        float closestDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < allAI.Count; i++)
        {
            float currentDistance = GetRange(allAI[i].transform);

            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestIndex = i;
            }
        }

        if (closestIndex < 0)
            return null;
        else return
            allAI[closestIndex];
	}

	void Update()
	{
        refireCounter -= Time.deltaTime;

        if (CurrentBuildingState == BuildingState.Finished)
        {
            Target = FindTarget().transform;

            if (Target)
                OrientTurret();

            if (CanFire())
                FireProjectile();
        }
	}

    bool CanFire()
    {
        if (Target != null && GetRange(Target) < TowerRange && refireCounter <= 0)
            return true;
        else return false;
    }

    void FireProjectile()
    {
        refireCounter = fireDelay;

        if (projectilePrefab != null)
            Instantiate(projectilePrefab, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.LookRotation(-TurretObject.transform.forward));
    }

	void OrientTurret()
	{
		if (TurretObject == null || Target == null)
			return;

        Vector3 lookDirection = transform.position - Target.position;
		lookDirection.y = 0;

		TurretObject.transform.rotation = Quaternion.LookRotation (lookDirection);
	}
}
