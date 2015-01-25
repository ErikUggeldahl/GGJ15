using UnityEngine;
using System.Collections;

public class ArchitectHealth : BaseHealth {

    private ArchitectPawn pawn = null;
    public ArchitectPawn Pawn { get { return pawn; } }

    // Use this for initialization
    void Start()
    {
        SetStartingHP();
    }

    public void Initialize(ArchitectPawn aPawn)
    {
        pawn = aPawn;
    }

    protected override void Die()
    {
        if(pawn.totemSegments.Count == 1)
        {
            Game.Instance.GameOver();
        }
        else
        {
            Respawn();
        }

        pawn.RemoveTotemSegment();
    }
}
