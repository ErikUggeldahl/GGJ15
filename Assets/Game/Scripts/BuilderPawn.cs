using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MPInput;

public class BuilderPawn : MonoBehaviour, IHealth
{
    public float MovementDrag = 2.0f;
    public float MovementForce = 20.0f;
    public float LookForce = 5.0f;

    private BuilderInput builderMovementScript = null;
    public BuilderInput BuilderMovementScript { get { return builderMovementScript; } }

    private bool isHoldingItem = false;
    public bool IsHoldingItem { get { return isHoldingItem; } }

    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

    private bool isFiring = false;
    public bool IsFiring { get { return isFiring; } }

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
    }

    public void Fire()
    {
        if (!IsHoldingItem)
        {
            if (!IsFiring)
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

                    if (nextDistance < smallestDistance)
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
            CurrentHeldResource.Drop();
            CurrentHeldResource = null;
        }
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

    private IEnumerator FiringCycle(float aWeaponFireAnimationTime)
    {
        do
        {
            // Fire projectile.
            Debug.Log("Firing projectile");

            yield return new WaitForSeconds(aWeaponFireAnimationTime);

        } while (IsFiring == true);
    }
}