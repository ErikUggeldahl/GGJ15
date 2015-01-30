using UnityEngine;
using System.Collections;

public class BuilderHealth : BaseHealth {

    private BuilderPawn pawn = null;
    public BuilderPawn Pawn { get { return pawn; } }

	public delegate void PlayerDeathDel();
	public event PlayerDeathDel onPlayerDeath;

	// Use this for initialization
	void Start () 
	{
		SetStartingHP();
	}

    public void Initialize(BuilderPawn aPawn)
    {
        pawn = aPawn;
    }

	protected override void Die ()
	{
		base.Die ();
		onPlayerDeath.Invoke();
	}
}
