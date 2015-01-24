using UnityEngine;
using System.Collections;

public class EagleEyeCamera : MonoBehaviour
{
    private float highestDistance = 0.0f;

    void Update()
    {
        highestDistance = 0.0f;

        for(int i = 0; i < Game.Instance.BuilderPawns.Count; i++)
        {
            if (Vector3.Distance(Game.Instance.BuilderPawns[i].transform.position, Game.Instance.ArchitectPawn.transform.position) > highestDistance)
            {
                highestDistance = Vector3.Distance(Game.Instance.BuilderPawns[i].transform.position, Game.Instance.ArchitectPawn.transform.position);
                highestDistance = Mathf.Clamp(highestDistance, 10.0f, 100.0f);
                highestDistance = highestDistance / 100.0f;

                highestDistance = -((-highestDistance - 10.0f) / (25.0f - 10.0f));
                Debug.Log(highestDistance);
            }
        }
    }

    void LateUpdate()
    {
        this.transform.position = new Vector3(Game.Instance.ArchitectPawn.transform.position.x, this.transform.position.y, this.transform.position.z);

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, highestDistance * 0.7f, 5.0f * Time.deltaTime);
    }
}