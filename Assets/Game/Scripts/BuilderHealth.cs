using UnityEngine;
using System.Collections;

public class BuilderHealth : BaseHealth {

    private BuilderPawn pawn = null;
    public BuilderPawn Pawn { get { return pawn; } }

	// Use this for initialization
	void Start () 
	{
		SetStartingHP();
	}

    public void Initialize(BuilderPawn aPawn)
    {
        pawn = aPawn;
    }
}
