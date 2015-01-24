using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArchitectPawn : MonoBehaviour
{
    int totemHeight = 3;

    public GameObject totemSegmentPrefab;
    List<Transform> totemSegments = new List<Transform>();

    private int health;
    public int Health { set { health = value; } get { return health; } }

    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

    void Start()
    {
        UpdateTotemSegmentVisuals();
    }

    void UpdateTotemSegmentVisuals()
    {
        if (totemSegments.Count < totemHeight)
        {
            for (int i = totemSegments.Count; i < totemHeight; i++)
            {
                GameObject newSegment = Instantiate(totemSegmentPrefab) as GameObject;

                newSegment.transform.SetParent(this.transform);
                newSegment.transform.localPosition = new Vector3(0, i, 0);

                totemSegments.Add(newSegment.transform);
            }
        }
        else if (totemSegments.Count > totemHeight)
        {
            for (int i = totemHeight; i < totemSegments.Count; i++)
            {
                Destroy(totemSegments[i].gameObject);
                totemSegments.RemoveAt(i);
            }
        }
    }

    void AddTotemSegment()
    {
        totemHeight++;
        UpdateTotemSegmentVisuals();
    }

    void RemoveTotemSegment()
    {
        totemHeight--;
        UpdateTotemSegmentVisuals();
    }

    public void TakeDamage(int aDamage)
    {
        if (!IsDead)
        {
            Health -= aDamage;

            if (Health <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        isDead = true;
    }

    public void Respawn()
    {
        isDead = false;
    }
}
