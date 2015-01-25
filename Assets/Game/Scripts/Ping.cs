using UnityEngine;
using System.Collections;

public class Ping : MonoBehaviour 
{
    public Transform mesh;

    public float bounceSpeed = 5.0f;
    public float bounceMagnitude = 1.0f;

    public float lifetime = 3.0f;

    float localTime = 0;

    public void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
            Destroy(this.gameObject);
        localTime += Time.deltaTime;
        float Y = 1 + Mathf.Sin(localTime * bounceSpeed) * bounceMagnitude;
        mesh.transform.localPosition = new Vector3(0, Y, 0);
    }
}
