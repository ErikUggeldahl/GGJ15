using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MPInput;

public class BuilderPawn : MonoBehaviour
{
    public GameObject SpearPrefab;

    public float MovementDrag = 2.0f;
    public float MovementForce = 20.0f;
    public float LookForce = 5.0f;

    private BuilderInput builderMovementScript = null;
    public BuilderInput BuilderMovementScript { get { return builderMovementScript; } }

    private BuilderHealth builderHealthScript = null;
    public BuilderHealth BuilderHealthScript { get { return builderHealthScript; } }

    private bool isHoldingItem = false;
    public bool IsHoldingItem { get { return isHoldingItem; } }

    private bool isFiring = false;
    public bool IsFiring { get { return isFiring; } }

    // Penalty from 0.0f - 1.0f (1% to 100%)
    public float movementPenaltyPercent = 0;
    public float MovementPenaltyPercent { get { return movementPenaltyPercent; } }

    public List<HarvestableResource> NearbyResources = new List<HarvestableResource>();
    public List<Building> NearbyBuildings = new List<Building>();

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

    private int health = 0;
    public int Health { set { health = value; } get { return health; } }

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

        builderHealthScript = this.gameObject.AddComponent<BuilderHealth>();
        builderHealthScript.Initialize(this);
       // builderHealthScript.Initialize(this, aInputDeviceInfo);
    }

    public void Fire()
    {
        if (!IsHoldingItem)
        {
            if (!IsFiring && !isFiringCoroutineActive)
            {
                isFiring = true;
                StartCoroutine(FiringCycle(1.5f));
            }
        }
    }

    public void CeaseFire()
    {
        if (IsFiring)
        {
            isFiring = false;
        }
    }

    public void PickupItem()
    {
        if (!IsFiring)
        {
            if (NearbyResources.Count > 0)
            {
                HarvestableResource nearestResource = NearbyResources[0];

                for (int i = 1; i < NearbyResources.Count; i++)
                {
                    float smallestDistance = Vector3.Distance(nearestResource.transform.position, this.transform.position);
                    float nextDistance = Vector3.Distance(NearbyResources[i].transform.position, this.transform.position);

                    if (nextDistance < smallestDistance && !nearestResource.IsHeld)
                    {
                        nearestResource = NearbyResources[i];
                    }
                }

                if (nearestResource != null)
                {
                    nearestResource.Pickup(this);
                    CurrentHeldResource = nearestResource;
                }
            }
        }
    }

    public void DropItem()
    {
        if (IsHoldingItem)
        {
            if (NearbyBuildings.Count > 0)
            {
                Building nearestBuilding = NearbyBuildings[0];

                for (int i = 1; i < NearbyBuildings.Count; i++)
                {
                    float smallestDistance = Vector3.Distance(nearestBuilding.transform.position, this.transform.position);
                    float nextDistance = Vector3.Distance(NearbyResources[i].transform.position, this.transform.position);

                    if (nextDistance < smallestDistance && nearestBuilding.CurrentBuildingState == Building.BuildingState.UnderConstruction)
                    {
                        nearestBuilding = NearbyBuildings[i];
                    }
                }

                if (nearestBuilding != null)
                {
                    NearbyResources.Remove(CurrentHeldResource);
                    nearestBuilding.AddResource(CurrentHeldResource);
                    CurrentHeldResource = null;
                }
                else
                {
                    CurrentHeldResource.Drop();
                    CurrentHeldResource = null;
                }
            }
            else
            {
                CurrentHeldResource.Drop();
                CurrentHeldResource = null;
            }
        }
    }

    private bool isFiringCoroutineActive = false;
    private IEnumerator FiringCycle(float aWeaponFireAnimationTime)
    {
        isFiringCoroutineActive = true;

        do
        {
            GameObject projectile = GameObject.Instantiate(SpearPrefab, this.transform.position, this.transform.rotation) as GameObject;
            projectile.GetComponent<Projectile>().Initialize(DamageSource.Builder, this);

            yield return new WaitForSeconds(aWeaponFireAnimationTime);

        } while (IsFiring == true);

        isFiringCoroutineActive = false;
    }
}