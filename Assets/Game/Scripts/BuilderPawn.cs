using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MPInput;

public class BuilderPawn : MonoBehaviour
{
    public GameObject heldSpearObject;

    public Ping pingPrefab = null;

    public GameObject SpearPrefab;

    public GameObject MeshObject;

    public Transform CarryPoint;

    public float FireRate = 0.5f;

    public float MovementDrag = 2.0f;
    public float MovementForce = 20.0f;
    public float LookForce = 5.0f;

    private BuilderInput builderMovementScript = null;
    public BuilderInput BuilderMovementScript { get { return builderMovementScript; } }

    private BuilderHealth builderHealthScript = null;
    public BuilderHealth BuilderHealthScript { get { return builderHealthScript; } }

    private bool isHoldingItem = false;
    public bool IsHoldingItem { get { return isHoldingItem; } }

    public bool isHoldingPlayer = false;
    public bool IsHoldingPlayer { get { return isHoldingPlayer; } }

    public bool isBeingHeld = false;
    public bool IsBeingHeld { get { return isBeingHeld; } }

    private bool isFiring = false;
    public bool IsFiring { get { return isFiring; } }

    // Penalty from 0.0f - 1.0f (1% to 100%)
    public float movementPenaltyPercent = 0;
    public float MovementPenaltyPercent { get { return movementPenaltyPercent; } }

    public List<HarvestableResource> NearbyResources = new List<HarvestableResource>();
    public List<Building> NearbyBuildings = new List<Building>();
    public BuilderPawn NearbyPlayer = null;

    public Material[] builderMaterials;
    public Mesh[] builderMeshes;

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

    private BuilderPawn currentHeldPlayer = null;
    public BuilderPawn CurrentHeldPlayer
    {
        set
        {
            if (value == null) isHoldingPlayer = false;
            else isHoldingPlayer = true;

            currentHeldPlayer = value;
        }
        get
        {
            return currentHeldPlayer;
        }
    }

    private BuilderPawn currentOwner = null;
    public BuilderPawn CurrentOwner
    {
        set
        {
            if (value == null) isBeingHeld = false;
            else isBeingHeld = true;

            currentOwner = value;
        }
        get
        {
            return currentOwner;
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

        UpdateBuilderHop();


        Quaternion targetRotation = Quaternion.identity;
        if (!builderHealthScript.IsAlive)
            targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.right);

        MeshObject.transform.localRotation = Quaternion.RotateTowards(MeshObject.transform.localRotation, targetRotation, 360 * Time.deltaTime);
    }

    public float hopVelocity = 10.0f;
    public float hopGravity = -9.8f;

    public float currentHopVelocity = 0.0f;

    void UpdateBuilderHop()
    {
        if (builderHealthScript != null && builderHealthScript.IsAlive)
        {
            if (MeshObject.transform.localPosition.y <= 0)
            {
                if (BuilderMovementScript.xAxisMove != 0 || BuilderMovementScript.yAxisMove != 0)
                {
                    currentHopVelocity = hopVelocity;
                }
                else
                {
                    currentHopVelocity = 0;
                    MeshObject.transform.localPosition = Vector3.Scale(MeshObject.transform.localPosition, new Vector3(1, 0, 1));
                }
            }
            else
            {
                currentHopVelocity -= Time.deltaTime * hopGravity;
            }

            MeshObject.transform.localPosition = MeshObject.transform.localPosition + Vector3.up * currentHopVelocity * Time.deltaTime;
        }
    }

    public void Initialize(MP_InputDeviceInfo aInputDeviceInfo)
    {
        builderMovementScript = this.gameObject.AddComponent<BuilderInput>();
        builderMovementScript.Initialize(this, aInputDeviceInfo);

        builderHealthScript = this.gameObject.AddComponent<BuilderHealth>();
        builderHealthScript.Initialize(this);
       // builderHealthScript.Initialize(this, aInputDeviceInfo);

        MeshObject.renderer.material = builderMaterials[aInputDeviceInfo.Index];
        MeshObject.GetComponent<MeshFilter>().mesh = builderMeshes[aInputDeviceInfo.Index];
    }

    public void Fire()
    {
        if (!IsHoldingItem && !IsFiring)
        {
            if (!IsFiring && !isFiringCoroutineActive)
            {
                isFiring = true;
                StartCoroutine(FiringCycle(FireRate));
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
            if (!IsHoldingItem && !IsHoldingPlayer && NearbyPlayer != null)
            {
                NearbyPlayer.Hold(this);
                CurrentHeldPlayer = NearbyPlayer;
            }
            else
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
    }

    public void DropItem()
    {
        if (IsHoldingItem)
        {
            GridPosition currentPosition = GameGrid.Instance.WorldToGridPosition(CarryPoint.position);
            GridSquare gridSquare = GameGrid.Instance.GetGridSquare(currentPosition);

            if (gridSquare.ResidingObject == null)
            {
                CurrentHeldResource.Drop();
                CurrentHeldResource = null;
                return;
            }

            Building hoveredBuilding = gridSquare.ResidingObject.GetComponent<Building>();

            if (hoveredBuilding != null)
            {
                if (hoveredBuilding.CurrentBuildingState == Building.BuildingState.Finished)
                    return;
                else
                {
                    // Building under construction
                    NearbyResources.Clear();
                    hoveredBuilding.ConsumeResource(CurrentHeldResource);
                    CurrentHeldResource = null;
                }
            }
            else
                return;
        }
        else if(IsHoldingPlayer)
        {
            CurrentHeldPlayer.Drop();
            CurrentHeldPlayer = null;
        }
    }

    public void Hold(BuilderPawn aPawn)
    {
        CurrentOwner = aPawn;
    }

    public void Drop()
    {
        CurrentOwner = null;
    }

    private bool isFiringCoroutineActive = false;
    private IEnumerator FiringCycle(float aWeaponFireAnimationTime)
    {
        isFiringCoroutineActive = true;

        do
        {
            GameObject projectile = GameObject.Instantiate(SpearPrefab, this.transform.position + Vector3.up * 0.5f, this.transform.rotation) as GameObject;
            projectile.GetComponent<Projectile>().Initialize(DamageSource.Builder, this);
            heldSpearObject.SetActive(false);
            yield return new WaitForSeconds(aWeaponFireAnimationTime);

        } while (IsFiring == true && builderHealthScript.IsAlive);
        heldSpearObject.SetActive(true);
        isFiringCoroutineActive = false;
    }

    protected virtual void OnTriggerEnter(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
           // if (aCollider.gameObject.GetComponent<BuilderPawn>() != this)
           // {
                BuilderPawn builderPawn = aCollider.gameObject.GetComponent<BuilderPawn>();

                NearbyPlayer = builderPawn;
            //}
        }
    }

    protected virtual void OnTriggerExit(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<BuilderPawn>() != null)
        {
            //if (aCollider.gameObject.GetComponent<BuilderPawn>() != this)
            //{
                NearbyPlayer = null;
           // }
        }
    }
}