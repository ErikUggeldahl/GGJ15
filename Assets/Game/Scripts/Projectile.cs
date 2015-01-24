using UnityEngine;
using System.Collections;

public enum DamageSource
{
    None,

    Builder,
    Architect,
    Enemy
}

public class Projectile : MonoBehaviour
{
    public int Damage;
    public int Drag;
    public int MoveForce;
    public int DestroyDistance;

    private DamageSource currentSource = DamageSource.None;
    public DamageSource CurrentSource { get { return currentSource; } }

    private MonoBehaviour sourceObject;
    public MonoBehaviour SourceObject { get { return sourceObject; } }

    public void Initialize(DamageSource aSource, MonoBehaviour aSourceObject)
    {
        currentSource = aSource;
        sourceObject = aSourceObject;
    }

    void Update()
    {
        if(Vector3.Distance(this.transform.position, sourceObject.transform.position) >= DestroyDistance)
        {
            Destroy(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        this.gameObject.rigidbody.drag = Drag;
        this.gameObject.rigidbody.AddForce(this.transform.forward * MoveForce);
	}
}
