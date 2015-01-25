using UnityEngine;
using System.Collections;

public class Bobble : MonoBehaviour 
{
    public Transform MeshTransform;

    public float frequency = 5.0f;
    public float angle = 5.0f;

    Vector2 currentPosition;
    Vector2 currentVelocity;

    public void ApplyImpulse(Vector3 Direction)
    {
        Vector3 localDirection = MeshTransform.InverseTransformDirection(Direction);
        currentVelocity += new Vector2(localDirection.z, -localDirection.x) * 200;
    }

    void Update()
    {
        Vector2 targetVector = new Vector3(Mathf.Sin(Time.time * frequency) * angle, 0);


        // Spring
        currentVelocity += (targetVector - currentPosition) * Time.deltaTime * 100;

        // Drag
        currentVelocity -= currentVelocity * Time.deltaTime * 8;

        currentPosition += currentVelocity * Time.deltaTime;
        MeshTransform.localEulerAngles = new Vector3(currentPosition.x, 0, currentPosition.y);
    }
}
