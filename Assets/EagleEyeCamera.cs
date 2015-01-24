using UnityEngine;
using System.Collections;

public class EagleEyeCamera : MonoBehaviour
{
    void LateUpdate()
    {
        this.transform.position = new Vector3(Game.Instance.ArchitectPawn.transform.position.x, this.transform.position.y, this.transform.position.z);


    }
}