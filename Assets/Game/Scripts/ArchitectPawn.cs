using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArchitectPawn : Building
{
    int totemHeight = 3;

    public GameObject totemSegmentPrefab;
    public List<Transform> totemSegments = new List<Transform>();

    private int health;
    public int Health { set { health = value; } get { return health; } }

    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

    private ArchitectHealth architectHealthScript = null;
    public ArchitectHealth ArchitectHealthScript { get { return architectHealthScript; } }

    void Start()
    {
        architectHealthScript = this.gameObject.AddComponent<ArchitectHealth>();
        architectHealthScript.Initialize(this);

        UpdateTotemSegmentVisuals();

        var gridPos = GameGrid.Instance.WorldToGridPosition(this.transform.position);
        var gridWorldPos = GameGrid.Instance.GridToWorldSpace(gridPos);
        var gridSquare = GameGrid.Instance.GetGridSquare(gridPos);

        if (gridSquare.ResidingObject == null)
        {
            gridSquare.ResidingObject = this.gameObject;
        }
    }

    void UpdateTotemSegmentVisuals()
    {
        if (totemSegments.Count < totemHeight)
        {
            for (int i = totemSegments.Count; i < totemHeight; i++)
            {
                GameObject newSegment = Instantiate(totemSegmentPrefab) as GameObject;

                newSegment.transform.SetParent(this.transform);
                newSegment.transform.localPosition = new Vector3(0, i, 0);

                totemSegments.Add(newSegment.transform);
            }
        }
        else if (totemSegments.Count > totemHeight && totemSegments.Count > 0)
        {
            for (int i = totemHeight; i < totemSegments.Count; i++)
            {
                Destroy(totemSegments[i].gameObject);
                totemSegments.RemoveAt(i);
            }
        }
    }

    public void AddTotemSegment()
    {
        totemHeight++;
        UpdateTotemSegmentVisuals();
    }

    public void RemoveTotemSegment()
    {
        totemHeight--;
        UpdateTotemSegmentVisuals();
    }

    public void TakeDamage(int aDamage)
    {
        if (!IsDead)
        {
            Health -= aDamage;

            if (Health <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        isDead = true;
    }

    public void Respawn()
    {
        isDead = false;
    }

    protected override void OnTriggerEnter(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
            BuilderPawn builderPawn = aCollider.gameObject.GetComponent<BuilderPawn>();
            
            if(builderPawn.BuilderHealthScript.IsAlive)
            {
                if(builderPawn.IsHoldingPlayer)
                {
                    builderPawn.CurrentHeldPlayer.BuilderHealthScript.Respawn();

                    builderPawn.DropItem();
                }
                else
                {
                    builderPawn.NearbyBuildings.Add(this);
                }
            }
        }
    }

    protected override void OnTriggerExit(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
            BuilderPawn builderPawn = aCollider.gameObject.GetComponent<BuilderPawn>();

            if(builderPawn.NearbyBuildings.Contains(this))
                builderPawn.NearbyBuildings.Remove(this);
        }
    }

    protected override void ConstructBuilding()
    {
        AddTotemSegment();
        currentWood = 0;
        currentStone = 0;
    }
}
