using UnityEngine;
using System.Collections;

interface IHealth
{
    int Health
    {
        get;
        set;
    }

    void Respawn();

    void TakeDamage(int aDamage);

    void Die();
}