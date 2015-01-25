using UnityEngine;
using System.Collections;

public class EagleEyeCamera : MonoBehaviour
{
    private float highestDistance = 0.0f;
	private float cameraFOV = 0.0f;
	private float cameraFOVPercentage = 0.0f;

    void Update()
    {
        highestDistance = 0.0f;

        for(int i = 0; i < Game.Instance.BuilderPawns.Count; i++)
        {
            if (Vector3.Distance(Game.Instance.BuilderPawns[i].transform.position, Game.Instance.ArchitectPawn.transform.position) > highestDistance)
            {
                highestDistance = Vector3.Distance(Game.Instance.BuilderPawns[i].transform.position, Game.Instance.ArchitectPawn.transform.position);
            }
        }

		Debug.Log(highestDistance);

		cameraFOVPercentage = Mathf.Clamp(highestDistance, 5.0f, 30.0f);
		cameraFOVPercentage = (cameraFOVPercentage - 5.0f) / (30.0f - 5.0f);
		cameraFOV = (cameraFOVPercentage * 2) + 8.0f;
    }

    void LateUpdate()
    {
        this.transform.position = new Vector3(Game.Instance.ArchitectPawn.transform.position.x, this.transform.position.y, this.transform.position.z);

		Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, cameraFOV, 5.0f * Time.deltaTime);
    }
}