using UnityEngine;
using System.Collections;

public class Bobble : MonoBehaviour 
{
    public Transform MeshTransform;

    public float frequency = 5.0f;
    public float angle = 5.0f;

    void Update()
    {
        Vector3 targetVector = new Vector3(Mathf.Sin(Time.time * frequency) * angle, 0, 0);

        MeshTransform.localEulerAngles = targetVector;
    }
}
