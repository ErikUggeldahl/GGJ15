using UnityEngine;
using System.Collections;

public class HarvestableResource : MonoBehaviour 
{
	public bool randomRotation = true;
	public float sizeVariance = 0.2f;


    protected bool isHeld = false;
    public bool IsHeld { get { return isHeld; } }

    protected BuilderPawn currentOwner = null;
    public BuilderPawn CurrentOwner
    {
        set
        {
            if (value == null) isHeld = false;
            else isHeld = true;

            currentOwner = value;
        }
        get
        {
            return currentOwner;
        }
    }

	protected virtual void Start()
	{
		float newScale = Random.Range(1.0f - (sizeVariance/2),1.0f + (sizeVariance/2));
		transform.localScale = Vector3.one * newScale;
		if (randomRotation)
			transform.Rotate(Vector3.up,Random.Range(0.0f,360.0f));
	}

    void Update()
    {
        if (isHeld)
        {
            this.transform.position = currentOwner.CarryPoint.position;
            this.transform.rotation = currentOwner.CarryPoint.rotation;
        }
    }

    public virtual void Pickup(BuilderPawn aPawn)
    {
        CurrentOwner = aPawn;
    }

    public virtual void Drop()
    {
        CurrentOwner = null;
        GridPosition gridPos = GameGrid.Instance.WorldToGridPosition(this.transform.position);
        this.transform.position = GameGrid.Instance.GridToWorldSpace(gridPos);
    }

    protected virtual void OnTriggerEnter(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
            BuilderPawn builderPawn = aCollider.gameObject.GetComponent<BuilderPawn>();

            builderPawn.NearbyResources.Add(this);
        }
    }

    protected virtual void OnTriggerExit(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
            BuilderPawn builderPawn = aCollider.gameObject.GetComponent<BuilderPawn>();

            builderPawn.NearbyResources.Remove(this);
        }
    }
}