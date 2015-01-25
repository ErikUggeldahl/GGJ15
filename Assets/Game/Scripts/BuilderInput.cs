using UnityEngine;
using System.Collections;

using MPInput;

public class BuilderInput : MonoBehaviour
{
    float pingTime = 0.0f;

    private MP_InputDeviceInfo inputDeviceInfo = null;
    public MP_InputDeviceInfo InputDeviceInfo { get { return inputDeviceInfo; } }

    private BuilderPawn pawn = null;
    public BuilderPawn Pawn { get { return pawn; } }

    public float xAxisMove = 0.0f;
    public float yAxisMove = 0.0f;
    public float xAxisLook = 0.0f;
    public float yAxisLook = 0.0f;

    private bool isPickupButtonPressed = false;
    private bool isFireButtonPressed = false;

    public void Initialize(BuilderPawn aPawn, MP_InputDeviceInfo aInputDeviceInfo)
    {
        pawn = aPawn;
        inputDeviceInfo = aInputDeviceInfo;
    }

    public void Update()
    {
        pingTime -= Time.deltaTime;

        // Poll input
        xAxisMove = MP_Input.GetAxis("MoveHorizontal", inputDeviceInfo, MP_eDeadzoneType.Radial);
        yAxisMove = MP_Input.GetAxis("MoveVertical", inputDeviceInfo, MP_eDeadzoneType.Radial);
        xAxisLook = MP_Input.GetAxis("AimHorizontal", inputDeviceInfo,MP_eDeadzoneType.Radial);
        yAxisLook = MP_Input.GetAxis("AimVertical", inputDeviceInfo,MP_eDeadzoneType.Radial);
        
        isFireButtonPressed = MP_Input.GetButton("BuilderFire", inputDeviceInfo);
        isPickupButtonPressed = MP_Input.GetButtonDown("Pickup/Drop", inputDeviceInfo);

        if (MP_Input.GetButtonDown("Ping", inputDeviceInfo) && CanPing())
        {
            Ping();
        }
    }

    bool CanPing()
    {
        return pingTime <= 0;
    }

    void Ping()
    {
        Ping newPint = Instantiate(pawn.pingPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as Ping;
        pingTime = newPint.lifetime;
        newPint.transform.SetParent(transform);
        // Force update right away
        newPint.Update();
    }

    public void FixedUpdate()
    {
        if (pawn.BuilderHealthScript.IsAlive)
        {
            // ** Commit actions based on input ** //

            // Movement
            Vector3 moveDirection = new Vector3(xAxisMove, 0, yAxisMove).normalized;
            Vector3 moveForce = (moveDirection * pawn.MovementForce) * (1.0f - pawn.MovementPenaltyPercent);
            this.gameObject.rigidbody.drag = pawn.MovementDrag;
            this.gameObject.rigidbody.AddForce(moveForce);

            // Aiming
            Vector3 targetLookDirection = new Vector3(xAxisLook, 0, yAxisLook).normalized;
            Vector3 currentLookDirection = Vector3.RotateTowards(this.transform.forward, targetLookDirection, pawn.LookForce * Time.fixedDeltaTime, 0.0f);
            this.gameObject.transform.rotation = Quaternion.LookRotation(currentLookDirection);

            // Buttons
            if (isPickupButtonPressed)
            {
                if (pawn.IsHoldingItem || pawn.IsHoldingPlayer)
                {
                    pawn.DropItem();
                }
                else
                {
                    pawn.PickupItem();
                }
            }

            if (isFireButtonPressed)
            {
                pawn.Fire();
            }
            else
            {
                pawn.CeaseFire();
            }
        }
        else
        {
            if (pawn.IsBeingHeld)
            {
                this.transform.position = pawn.CurrentOwner.CarryPoint.transform.position;
                this.transform.rotation = pawn.CurrentOwner.CarryPoint.transform.rotation;
            }
        }
    }
}