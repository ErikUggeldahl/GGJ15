using UnityEngine;
using System.Collections;

using MPInput;

public class BuilderInput : MonoBehaviour
{
    public float Drag = 2;

    private MP_InputDeviceInfo inputDeviceInfo = null;
    public MP_InputDeviceInfo InputDeviceInfo { get { return inputDeviceInfo; } }

    private BuilderPawn pawn = null;
    public BuilderPawn Pawn { get { return pawn; } }

    private float xAxisMove = 0.0f;
    private float yAxisMove = 0.0f;
    private float xAxisLook = 0.0f;
    private float yAxisLook = 0.0f;

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

        isPickupButtonPressed = MP_Input.GetButtonDown("BuilderFire", inputDeviceInfo);
        isFireButtonPressed = MP_Input.GetButtonDown("Pickup/Drop", inputDeviceInfo);
    }

    public void FixedUpdate()
    {
        // Commit actions based on input
        Vector3 moveDirection = new Vector3(xAxisMove, 0, yAxisMove).normalized;
        Vector3 moveForce = moveDirection * 20;

        this.gameObject.rigidbody.drag = Drag;
        this.gameObject.rigidbody.AddForce(moveForce);

        // Buttons
        if(isPickupButtonPressed)
        {
            if(pawn.IsHoldingItem)
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
    }
}