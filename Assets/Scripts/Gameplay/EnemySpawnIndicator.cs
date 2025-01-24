using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemySpawnIndicator : MonoBehaviour
{
    public async void Setup(int enemyId)
    {
        await UniTask.WaitForSeconds(1f);
        if (!this) return;

        SpawnManager.Instance.SpawnEnemyUnit(enemyId, transform.position);
    }
}
