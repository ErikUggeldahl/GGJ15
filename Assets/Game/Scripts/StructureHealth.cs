using UnityEngine;
using System.Collections;

public class StructureHealth : BaseHealth {

	// Use this for initialization
	void Start () 
	{
		SetStartingHP();
	}

	protected override void Die ()
	{
		base.Die ();
		Destroy(gameObject);
	}
}
