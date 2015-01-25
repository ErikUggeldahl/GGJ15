using UnityEngine;
using System.Collections;

public class SpearProjectile : Projectile
{
    protected virtual void OnTriggerEnter(Collider aCollider)
    {
        if (aCollider.gameObject.GetComponent<AIHealth>() != null)
        {
            AIHealth ai = aCollider.gameObject.GetComponent<AIHealth>();

			ai.TakeDamage(1, sourceObject.transform);

            Destroy(this.gameObject);
        }
    }
}
