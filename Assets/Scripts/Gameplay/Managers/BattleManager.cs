using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Cutscene,
    InGame,
    BetweenWaves,
}

public class BattleManager : Singleton<BattleManager>
{
    public GameState State { get; private set; }

    public int CurrentWave { get; private set; }

    public float StageTimer { get; private set; }

    protected override void HandleAwake()
    {
        CurrentWave = 0;
        StageTimer = 0f;
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
                // Kill all enemies and projectiles
                var enemyUnits = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemyUnits)
                {
                    enemy.GetComponent<EnemyUnit>().Kill();
                }

                // TODO
                var enemyProjectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");

                // TODO: Freeze / reset everything
                State = GameState.BetweenWaves;
                PlayerHudManager.Instance.DisplayEndWaveUI().Forget();
            }
        }
    }

    public void NextWave()
    {
        CurrentWave++;
        int timeForStage = DWave.GetDataById(CurrentWave).Value.WaveTime;
        StageTimer = timeForStage;

        SpawnManager.Instance.SpawnWave(CurrentWave);

        State = GameState.InGame;
    }

    public bool CheckIfPlayerPassed()
    {
        var dWave = DWave.GetDataById(CurrentWave).Value;

        // TODO: Replace this!
        //return dWave.CleanThreshold >= the_threshold;

        return true;
    }

    private void FinishGame(bool isWin)
    {

    }
}
