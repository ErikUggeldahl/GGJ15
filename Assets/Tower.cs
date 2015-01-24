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

	void OrientTurret()
	{
		if (TurretObject == null)
			return;

		Vector3 lookDirection = transform.position - TurretObject.position;
		lookDirection.y = 0;

		TurretObject.transform.rotation = Quaternion.LookRotation (lookDirection);
	}
}
