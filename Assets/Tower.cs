using UnityEngine;
using System.Collections;

public class Tower : Building
{
	public float TowerRange;

	public Transform TurretObject;

    public Transform Target;


	GameObject FindTarget()
	{

        return null;
	}

	void Update()
	{
		if (CurrentBuildingState == BuildingState.Finished)
			OrientTurret ();
	}

    void FireProjectile()
    {

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
