using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MPInput;

public class BuilderPawn : MonoBehaviour
{
    public float MovementDrag = 2.0f;
    public float MovementForce = 20.0f;
    public float LookForce = 5.0f;

    private BuilderInput builderMovementScript = null;
    public BuilderInput BuilderMovementScript { get { return builderMovementScript; } }

    private bool isHoldingItem = false;
    public bool IsHoldingItem { get { return isHoldingItem; } }

    // Penalty from 0.0f - 1.0f (1% to 100%)
    public float movementPenaltyPercent = 0;
    public float MovementPenaltyPercent { get { return movementPenaltyPercent; } }

    public List<HarvestableResource> NearbyResources = new List<HarvestableResource>();

    private HarvestableResource currentHeldResource = null;
    public HarvestableResource CurrentHeldResource
    {
        set
        {
            if(value == null) isHoldingItem = false;
            else isHoldingItem = true;

            currentHeldResource = value;
        }
        get
        {
            return currentHeldResource;
        }
    }

    void Update()
    {
        if(isHoldingItem)
        {
            movementPenaltyPercent = 0.5f;
        }
        else
        {
            movementPenaltyPercent = 0.0f;
        }
    }

    public void Initialize(MP_InputDeviceInfo aInputDeviceInfo)
    {
        builderMovementScript = this.gameObject.AddComponent<BuilderInput>();
        builderMovementScript.Initialize(this, aInputDeviceInfo);
    }

    public void Fire()
    {

    }

    public void PickupItem()
    {
        if(NearbyResources.Count > 0)
        {
            HarvestableResource nearestResource = NearbyResources[0];

            for(int i = 1; i < NearbyResources.Count; i++)
            {
                float smallestDistance = Vector3.Distance(nearestResource.transform.position, this.transform.position);
                float nextDistance = Vector3.Distance(NearbyResources[i].transform.position, this.transform.position);

                if (nextDistance < smallestDistance)
                {
                    nearestResource = NearbyResources[i];
                }
            }

            if(nearestResource != null)
            {
                nearestResource.Pickup(this);
                CurrentHeldResource = nearestResource;
            }
        }
    }

    public void DropItem()
    {
        CurrentHeldResource.Drop();
        CurrentHeldResource = null;
    }
}