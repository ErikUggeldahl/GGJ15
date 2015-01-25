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
    public int ProjectileSpeed;
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

    void Start()
    {
        this.gameObject.rigidbody.velocity = this.transform.forward * ProjectileSpeed;
    }

    protected virtual void Update()
    {
        if(Vector3.Distance(this.transform.position, sourceObject.transform.position) >= DestroyDistance)
        {
            Destroy(this.gameObject);
        }
    }
	
}
