using UnityEngine;
using System.Collections;

using MPInput;

public class BuilderInput : MonoBehaviour
{
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
        // Poll input
        xAxisMove = MP_Input.GetAxis("MoveHorizontal", inputDeviceInfo);
        yAxisMove = MP_Input.GetAxis("MoveVertical", inputDeviceInfo);
        xAxisLook = MP_Input.GetAxis("AimHorizontal", inputDeviceInfo);
        yAxisLook = MP_Input.GetAxis("AimVertical", inputDeviceInfo);
        
        isFireButtonPressed = MP_Input.GetButton("BuilderFire", inputDeviceInfo);
        isPickupButtonPressed = MP_Input.GetButtonDown("Pickup/Drop", inputDeviceInfo);
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
                if (pawn.IsHoldingItem)
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
    }
}