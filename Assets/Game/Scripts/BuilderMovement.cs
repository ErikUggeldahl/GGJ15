using UnityEngine;
using System.Collections;

using MPInput;

public class BuilderMovement : MonoBehaviour
{
    float xAxis = 0.0f;
    float yAxis = 0.0f;

    public void LOL()
    {
        xAxis = MP_Input.GetAxis("MoveHorizontal", new MP_InputDeviceInfo(MP_eInputType.Controller, 0));
        yAxis = MP_Input.GetAxis("MoveVertical", new MP_InputDeviceInfo(MP_eInputType.Controller, 0));

        
    }
}
