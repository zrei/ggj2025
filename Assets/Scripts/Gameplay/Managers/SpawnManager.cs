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

    private WaveData m_CurrentWaveData;

    public DamageText SpawnDamageText(Vector2 spawnPosition)
    {
        return Instantiate(DamageTextPrefab, spawnPosition, Quaternion.identity);
    }

    public void SpawnWave(int waveId)
    {
        m_CurrentWaveData = DWave.GetDataById(waveId).Value;
        var spawnDataList = DWaveSpawn.GetDataById(waveId);

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
        enemyUnit.Setup(enemyId, m_CurrentWaveData.AttackMultiplier, m_CurrentWaveData.HpMultiplier,
            m_CurrentWaveData.MovementSpeedMultiplier, m_CurrentWaveData.ProjectileSpeedMult);
    }
}
