using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [field: SerializeField, Header("Prefabs")]
    private DamageText DamageTextPrefab { get; set; }

    public DamageText SpawnDamageText(Vector2 spawnPosition)
    {
        return Instantiate(DamageTextPrefab, spawnPosition, Quaternion.identity);
    }

    public void SpawnEnemy(Vector2 spawnPosition)
    {
        // TODO
    }
}
