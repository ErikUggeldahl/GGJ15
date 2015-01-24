using UnityEngine;
using System.Collections;

public class HarvestableResource : MonoBehaviour 
{
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

    public virtual void Pickup(BuilderPawn aPawn)
    {
        CurrentOwner = aPawn;
    }

    public virtual void Drop()
    {
        CurrentOwner = null;
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
