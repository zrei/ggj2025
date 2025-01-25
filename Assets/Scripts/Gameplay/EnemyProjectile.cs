using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public void Despawn()
    {
        Destroy(gameObject);
    }
}
