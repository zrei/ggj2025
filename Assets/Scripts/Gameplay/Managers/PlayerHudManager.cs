using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHudManager : Singleton<PlayerHudManager>
{
    [field: SerializeField]
    private Slider PlayerHealthSlider { get; set; }

    [field: SerializeField, Header("Stage Timer")]
    private GameObject StageTimerParent {  get; set; }

    [field: SerializeField]
    private TextMeshProUGUI StageTimerText { get; set; }

    [field: SerializeField, Header("Clean Threshold")]
    private TextMeshProUGUI CleanThresholdText { get; set; }

    [field: SerializeField, Header("Wave Start Display")]
    private Animator WaveStartAnimator { get; set; }

    [field: SerializeField, Header("Lose Game Display")]
    private GameObject LoseGameDisplayParent { get; set; }

    [field: SerializeField]
    private Button TryAgainButton { get; set; }

    [field: SerializeField]
    private Button MainMenuButton { get; set; }

    [field: SerializeField, Header("Sounds")]
    private AudioSource AudioSource { get; set; }

    [field: SerializeField]
    private AudioClip WhistleAudioClip { get; set; }

    [field: SerializeField]
    private AudioClip SadAudioClip { get; set; }

    protected override void HandleAwake()
    {
        Player.OnPlayerHealthValueChanged += OnPlayerHealthValueChanged;

        TryAgainButton.onClick.RemoveAllListeners();
        TryAgainButton.onClick.AddListener(OnTryAgainButtonClick);
        MainMenuButton.onClick.RemoveAllListeners();
        MainMenuButton.onClick.AddListener(OnMainMenuButtonClick);

        PlayerHealthSlider.maxValue = GlobalEvents.Player.PlayerMaxHealth;
        PlayerHealthSlider.value = GlobalEvents.Player.PlayerMaxHealth;
    }

    protected override void HandleDestroy()
    {
        Player.OnPlayerHealthValueChanged -= OnPlayerHealthValueChanged;
    }

    private void Update()
    {
        if (BattleManager.Instance.State == GameState.InGame)
        {
            StageTimerText.text = ConvertTimerToDisplay(BattleManager.Instance.StageTimer);
            // TODO
            // CleanThresholdText.text = $"{}%"
        }
    }

    public void StartGame()
    {
        DisplayStartWaveUI().Forget();
    }

    private async UniTask DisplayStartWaveUI()
    {
        // TODO: Show "Get Ready" text

        // We get the time for the stage first to display
        int nextWaveNumber = BattleManager.Instance.CurrentWave + 1;
        int timeForStage = DWave.GetDataById(nextWaveNumber).Value.WaveTime;
        StageTimerText.text = ConvertTimerToDisplay(timeForStage);

        // Some buffer for the next wave display to animate
        WaveStartAnimator.Play("WaveStartMoveIn");
        await UniTask.WaitForSeconds(1.5f);
        if (!this) return;

        WaveStartAnimator.Play("WaveStartMoveOut");
        await UniTask.WaitForSeconds(1.5f);
        if (!this) return;

        BattleManager.Instance.NextWave();

        await UniTask.CompletedTask;
    }

    private string ConvertTimerToDisplay(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        string minutesDisplay = minutes < 10 ? $"0{minutes}" : $"{minutes}";
        string secondsDisplay = seconds < 10 ? $"0{seconds}" : $"{seconds}";

        return $"{minutesDisplay}:{secondsDisplay}";
    }

    public async UniTask DisplayEndWaveUI()
    {
        // TODO: Display the wave end UI

        AudioSource.PlayOneShot(WhistleAudioClip);
        bool wavePassed = BattleManager.Instance.CheckIfPlayerPassed();

        // Some buffer time between waves
        await UniTask.WaitForSeconds(1f);
        if (!this) return;

        if (wavePassed)
        {
            DisplayStartWaveUI().Forget();
        }
        else
        {
            AudioSource.PlayOneShot(SadAudioClip);
            LoseGameDisplayParent.SetActive(true);
        }

        await UniTask.CompletedTask;
    }

    private void OnPlayerHealthValueChanged(int value)
    {
        PlayerHealthSlider.value = value; 
    }

    private void OnTryAgainButtonClick()
    {
        // TODO

    }

    private void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
