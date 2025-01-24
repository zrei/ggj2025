using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Cutscene,
    InGame,
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
                PlayerHudManager.Instance.DisplayWaveComplete();
                State = GameState.Cutscene;
            }
        }
    }

    private void StartGame()
    {
        CurrentWave = 1;
    }

    private void NextWave()
    {

    }

    private void CheckIfPlayerPassed()
    {

    }

    private void FinishGame(bool isWin)
    {

    }
}
