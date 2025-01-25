using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Cutscene,
    InGame,
    BetweenWaves,
}

public class BattleManager : Singleton<BattleManager>
{
    public GameState State { get; set; }

    public int CurrentWave { get; private set; }

    public float StageTimer { get; private set; }

    protected override void HandleAwake()
    {
        CurrentWave = 0;
        StageTimer = 0f;

        BeginGame();
    }

    private async void BeginGame()
    {
        if (!SceneManager.GetActiveScene().name.Equals("Gameplay"))
        {
            return;
        }

        await UniTask.WaitForSeconds(1f);
        if (!this) return;

        NextWave();
    }

    private void Update()
    {
        if (State == GameState.InGame)
        {
            if (StageTimer > 0f)
            {
                StageTimer -= Time.deltaTime;
            }
            else
            {
                State = GameState.BetweenWaves;

                // Kill all enemies and projectiles
                var enemyUnits = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemyUnits)
                {
                    enemy.GetComponent<EnemyUnit>().Kill();
                }
                var enemyProjectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
                foreach ( var enemyProjectile in enemyProjectiles)
                {
                    enemyProjectile.GetComponent<EnemyProjectile>().Despawn();
                }

                PlayerHudManager.Instance.DisplayEndWaveUI().Forget();
            }
        }
    }

    public void NextWave()
    {
        // TODO: Reset floors to neutral

        CurrentWave++;
        if (CurrentWave > DWave.GetAllData().Data.Count)
        {
            FinishGame();
            return;
        }

        int timeForStage = DWave.GetDataById(CurrentWave).Value.WaveTime;
        StageTimer = timeForStage;

        SpawnManager.Instance.SpawnWave(CurrentWave);

        State = GameState.InGame;
    }

    public bool CheckIfPlayerPassed()
    {
        return Mathf.CeilToInt(GridManager.Instance.GetCleanedPercentage) >= DWave.GetDataById(CurrentWave).Value.CleanThreshold;
    }

    private void FinishGame()
    {
        SceneManager.LoadScene("EndGame");
    }
}
