using UnityEngine;
using System.Collections;

using MPInput;

public class BuilderPawn : MonoBehaviour
{
    private BuilderInput builderMovementScript = null;
    public BuilderInput BuilderMovementScript { get { return builderMovementScript; } }

    private bool isHoldingItem = false;
    public bool IsHoldingItem { get { return isHoldingItem; } }

    public void CreatePawn(MP_InputDeviceInfo aInputDeviceInfo)
    {
        builderMovementScript = this.gameObject.AddComponent<BuilderInput>();
        builderMovementScript.Initialize(this, aInputDeviceInfo);


    }

    public void Fire()
    {

    }

    public void PickupItem()
    {

    }

    public void DropItem()
    {

    }
}
