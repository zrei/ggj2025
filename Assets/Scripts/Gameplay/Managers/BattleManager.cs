using Cysharp.Threading.Tasks;
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

    public WaveData WaveData { get; private set; }

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
                GlobalEvents.Waves.OnWaveEndEvent?.Invoke();

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
        GlobalEvents.Waves.OnWaveStartEvent?.Invoke();

        CurrentWave++;
        if (CurrentWave > DWave.GetAllData().Data.Count)
        {
            FinishGame();
            return;
        }

        WaveData = DWave.GetDataById(CurrentWave).Value;
        int timeForStage = WaveData.WaveTime;
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
