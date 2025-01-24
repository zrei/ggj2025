using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [field: SerializeField, Header("Prefabs")]
    private DamageText DamageTextPrefab { get; set; }

    [field: SerializeField]
    private EnemySpawnIndicator EnemySpawnIndicatorPrefab { get; set; }

    [field: SerializeField]
    private List<EnemyUnit> EnemyUnitPrefabs { get; set; }

    public DamageText SpawnDamageText(Vector2 spawnPosition)
    {
        return Instantiate(DamageTextPrefab, spawnPosition, Quaternion.identity);
    }

    public void SpawnWave(int id)
    {
        var spawnDataList = DWaveSpawn.GetDataById(id);

        foreach (var waveSpawnData in spawnDataList)
        {
            Vector2 spawnPosition = GridManager.Instance.GetWorldPositionOfTile(waveSpawnData.RowSpawnPoint, waveSpawnData.ColSpawnPoint);
            SpawnEnemyIndicator(waveSpawnData.EnemyId, spawnPosition, waveSpawnData.Delay).Forget();
        }
    }

    private async UniTask SpawnEnemyIndicator(int enemyId, Vector2 spawnPosition, float delay)
    {
        await UniTask.WaitForSeconds(delay);
        if (!this) return;

        var indicator = Instantiate(EnemySpawnIndicatorPrefab, spawnPosition, Quaternion.identity);
        indicator.Setup(enemyId);
    }

    public void SpawnEnemyUnit(int enemyId, Vector2 spawnPosition)
    {
        var enemyUnit = Instantiate(EnemyUnitPrefabs[enemyId], spawnPosition, Quaternion.identity);
        enemyUnit.Setup(enemyId);
    }
}
