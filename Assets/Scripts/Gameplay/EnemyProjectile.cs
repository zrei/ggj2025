using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    /*
     * Called by enemy behaviour
     */
    public void Setup(Vector2 direction)
    {

    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
