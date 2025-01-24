using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHudManager : Singleton<PlayerHudManager>
{
    [field: SerializeField]
    private Slider PlayerHealthBar { get; set; }

    [field: SerializeField, Header("Stage Timer")]
    private GameObject StageTimerParent {  get; set; }

    [field: SerializeField]
    private TextMeshProUGUI StageTimerText { get; set; }

    [field: SerializeField, Header("End Wave Display")]
    private GameObject EndWaveDisplayParent { get; set; }

    [field: SerializeField]
    private Button NextWaveButton { get; set; }

    [field: SerializeField]
    private Button TryAgainButton { get; set; }

    [field: SerializeField]
    private Button MainMenuButton { get; set; }

    [field: SerializeField, Header("Sounds")]
    private AudioSource AudioSource { get; set; }

    private AudioClip DrumrollAudioClip { get; set; }

    [field: SerializeField]
    private AudioClip CheerAudioClip { get; set; }

    [field: SerializeField]
    private AudioClip SadAudioClip { get; set; }

    protected override void HandleAwake()
    {
        NextWaveButton.onClick.RemoveAllListeners();
        NextWaveButton.onClick.AddListener(OnNextWaveButtonClick);
        TryAgainButton.onClick.RemoveAllListeners();
        TryAgainButton.onClick.AddListener(OnTryAgainButtonClick);
        MainMenuButton.onClick.RemoveAllListeners();
        MainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
    }

    private void Update()
    {
        if (BattleManager.Instance.State == GameState.InGame)
        {
            StageTimerText.text = ConvertTimerToDisplay(BattleManager.Instance.StageTimer);
        }
    }

    private async UniTask DisplayStartWaveUI()
    {
        // TODO: Show "Get Ready" text

        // We get the time for the stage first to display
        int nextWaveNumber = BattleManager.Instance.CurrentWave + 1;
        int timeForStage = DWave.GetDataById(nextWaveNumber).Value.WaveTime;

        await UniTask.WaitForSeconds(1f);
        if (!this) return;

        BattleManager.Instance.NextWave();
    }

    private string ConvertTimerToDisplay(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        string minutesDisplay = minutes < 10 ? $"0{minutes}" : $"{minutes}";
        string secondsDisplay = seconds < 10 ? $"0{seconds}" : $"{seconds}";

        return $"{minutesDisplay}:{secondsDisplay}";
    }

    public void DisplayEndWaveUI()
    {
        DisplayEndWavePanel().Forget();
    }

    private async UniTask DisplayEndWavePanel()
    {
        bool wavePassed = BattleManager.Instance.CheckIfPlayerPassed();

        // Play drumroll effect and display the result panel
        AudioSource.PlayOneShot(DrumrollAudioClip);
        EndWaveDisplayParent.SetActive(true);

        // TODO: Do the random numbers effect

        await UniTask.WaitForSeconds(2f);
        if (!this) return;

        if (wavePassed)
        {
            
        }
        else
        {

        }
    }

    private void OnNextWaveButtonClick()
    {
        EndWaveDisplayParent.SetActive(false);

        DisplayStartWaveUI().Forget();
    }

    private void OnTryAgainButtonClick()
    {
        // TODO: idk about the logic yet lol
    }

    private void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
