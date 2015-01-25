using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour 
{
    public float ascendSpeed = 10.0f;
    public float ascentTime = 2.0f;
    public float ascentTimeRemaining;

    void Start()
    {
        ascentTimeRemaining = ascentTime;
    }

	void Update () 
    {
        ascentTimeRemaining -= Time.deltaTime;
        if (ascentTimeRemaining <= 0)
            Destroy(this.gameObject);

        transform.position = transform.position + Vector3.up * ascendSpeed * Time.deltaTime;

        renderer.material.SetFloat("_Transparency", ascentTimeRemaining / ascentTime);
	}
}
