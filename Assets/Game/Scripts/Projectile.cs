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

    protected DamageSource currentSource = DamageSource.None;
    public DamageSource CurrentSource { get { return currentSource; } }

    protected MonoBehaviour sourceObject;
    public MonoBehaviour SourceObject { get { return sourceObject; } }

    public void Initialize(DamageSource aSource, MonoBehaviour aSourceObject)
    {
        currentSource = aSource;
        sourceObject = aSourceObject;
    }

    protected virtual void Update()
    {
        if(Vector3.Distance(this.transform.position, sourceObject.transform.position) >= DestroyDistance)
        {
            Destroy(this.gameObject);
        }
    }
	
	// Update is called once per frame
	protected virtual void FixedUpdate ()
    {
        this.gameObject.rigidbody.drag = Drag;
        this.gameObject.rigidbody.AddForce(this.transform.forward * MoveForce);
	}
}
