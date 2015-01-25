using UnityEngine;
using System.Collections;

public class ArchitectHealth : BaseHealth {

	// Use this for initialization
	void Start () 
	{
		SetStartingHP();
	}

	protected override void Die ()
	{
		base.Die ();
		Game.Instance.GameOver();
	}
}
